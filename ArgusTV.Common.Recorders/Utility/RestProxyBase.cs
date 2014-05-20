/*
 *	Copyright (C) 2007-2014 ARGUS TV
 *	http://www.argus-tv.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Nancy.Helpers;
using ArgusTV.Common.Logging;

namespace ArgusTV.Common.Recorders.Utility
{
    public abstract class RestProxyBase
    {
        /// <exclude />
        protected HttpClient _client;

        /// <exclude />
        public RestProxyBase(string baseUrl)
        {
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl + "/";
            }
            _client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true)
            {
                BaseAddress = new Uri(baseUrl)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        }

        /// <exclude />
        protected HttpRequestMessage NewRequest(HttpMethod method, string url, params object[] args)
        {
            if (url.StartsWith("/"))
            {
                url = url.Substring(1);
            }
            if (args != null && args.Length > 0)
            {
                List<object> encodedArgs = new List<object>();
                foreach (var arg in args)
                {
                    string urlArg;
                    if (arg is DateTime)
                    {
                        DateTime time = (DateTime)arg;
                        if (time.Kind == DateTimeKind.Unspecified)
                        {
                            urlArg = new DateTime(time.Ticks, DateTimeKind.Local).ToString("o");
                        }
                        urlArg = time.ToString("o");
                    }
                    else
                    {
                        urlArg = arg.ToString();
                    }
                    encodedArgs.Add(HttpUtility.UrlEncode(urlArg));
                }
                return new HttpRequestMessage(method, String.Format(url, encodedArgs.ToArray()));
            }
            return new HttpRequestMessage(method, url);
        }

        /// <exclude />
        protected void ExecuteAsync(HttpRequestMessage request)
        {
            try
            {
                var task = _client.SendAsync(request);
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Logger.Error(t.Exception.ToString());
                    }
                    else if (t.IsCompleted)
                    {
                        var r = t.Result;
                        if (r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            var error = SimpleJson.DeserializeObject<RestError>(r.Content.ReadAsStringAsync().Result);
                            Logger.Error(error.detail);
                        }
                        else
                        {
                            Logger.Error(r.ReasonPhrase);
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                Logger.Error(ex.ToString());
                EventLogger.WriteEntry(ex);
                throw new ApplicationException("An unexpected error occured.");
            }
        }

        /// <exclude />
        protected bool IsConnectionError(Exception ex)
        {
            var requestException = ex as HttpRequestException;
            if (requestException != null)
            {
                var webException = requestException.InnerException as WebException;
                if (webException != null)
                {
                    switch (webException.Status)
                    {
                        case System.Net.WebExceptionStatus.ConnectFailure:
                        case System.Net.WebExceptionStatus.NameResolutionFailure:
                        case System.Net.WebExceptionStatus.ProxyNameResolutionFailure:
                        case System.Net.WebExceptionStatus.RequestProhibitedByProxy:
                        case System.Net.WebExceptionStatus.SecureChannelFailure:
                        case System.Net.WebExceptionStatus.TrustFailure:
                            return true;
                    }
                }
            }
            return false;
        }

        /// <exclude />
        protected void Execute(HttpRequestMessage request, bool logError = true)
        {
            try
            {
                using (var response = ExecuteRequest(request, logError))
                {
                }
            }
            finally
            {
                request.Dispose();
            }
        }

        /// <exclude />
        protected T Execute<T>(HttpRequestMessage request, bool logError = true)
            where T : new()
        {
            try
            {
                using (var response = ExecuteRequest(request, logError))
                {
                    return DeserializeResponseContent<T>(response);
                }
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch(Exception ex)
            {
                if (logError)
                {
                    Logger.Error(ex.ToString());
                    EventLogger.WriteEntry(ex);
                }
                throw new ApplicationException("An unexpected error occured.");
            }
            finally
            {
                request.Dispose();
            }
        }

        /// <exclude />
        protected T ExecuteResult<T>(HttpRequestMessage request)
        {
            var data = Execute<SimpleResult<T>>(request);
            return data.result;
        }

        private HttpResponseMessage ExecuteRequest(HttpRequestMessage request, bool logError)
        {
            try
            {
                var response = _client.SendAsync(request).Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        var error = SimpleJson.DeserializeObject<RestError>(response.Content.ReadAsStringAsync().Result);
                        throw new ApplicationException(error.detail);
                    }
                    throw new ApplicationException(response.ReasonPhrase);
                }
                return response;
            }
            catch (AggregateException ex)
            {
                if (IsConnectionError(ex.InnerException))
                {
                    throw new RecorderNotFoundException(ex.InnerException.InnerException.Message);
                }
                throw;
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch(Exception ex)
            {
                if (logError)
                {
                    Logger.Error(ex.ToString());
                    EventLogger.WriteEntry(ex);
                }
                throw new ApplicationException("An unexpected error occured.");
            }
        }

        /// <exclude />
        protected static T DeserializeResponseContent<T>(HttpResponseMessage response)
            where T : new()
        {
            string content = response.Content.ReadAsStringAsync().Result;
            if (String.IsNullOrEmpty(content))
            {
                return default(T);
            }
            return SimpleJson.DeserializeObject<T>(content, new RecorderJsonSerializerStrategy());
        }

        protected class SimpleResult<T>
        {
            public T result { get; set; }
            public string errorMessage { get; set; }
        }

        private class RestError
        {
            public string detail { get; set; }
        }
    }

    /// <exclude />
    public static class HttpRequestMessageExtensions
    {
        /// <exclude />
        public static void AddBody(this HttpRequestMessage request, object body)
        {
            request.Content = new StringContent(
                SimpleJson.SerializeObject(body, new RecorderJsonSerializerStrategy()), Encoding.UTF8, "application/json");
        }

        /// <exclude />
        public static void AddParameter(this HttpRequestMessage request, string name, object value)
        {
            string url = request.RequestUri.OriginalString;
            url += (url.Contains("?") ? "&" : "?") + name + "=" + HttpUtility.UrlEncode(value.ToString());
            request.RequestUri = new Uri(url, UriKind.Relative);
        }
    }
}
