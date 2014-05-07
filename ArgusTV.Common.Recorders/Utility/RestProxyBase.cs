using System;
using System.Collections.Generic;
using RestSharp;
using ArgusTV.Common.Logging;

namespace ArgusTV.Common.Recorders.Utility
{
    public abstract class RestProxyBase
    {
        private RestClient _client;

        public RestProxyBase(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        protected RestRequest NewRequest(string url, Method method)
        {
            return new RestRequest(url, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new RecorderJsonSerializer()
            };
        }

        protected void ExecuteAsync(RestRequest request)
        {
            try
            {
                var response = _client.ExecuteAsync(request, r =>
                {
                    if (r.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            var error = SimpleJson.DeserializeObject<RestError>(r.Content);
                            Logger.Error(error.detail);
                        }
                        else
                        {
                            Logger.Error(r.ErrorMessage);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                EventLogger.WriteEntry(ex);
                throw new Exception("An unexpected error occured.");
            }
        }

        private bool IsConnectionError(Exception ex)
        {
            System.Net.WebException webException = ex as System.Net.WebException;
            if (ex != null)
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
            return false;
        }

        protected IRestResponse Execute(RestRequest request)
        {
            try
            {
                var response = _client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == 0 && IsConnectionError(response.ErrorException))
                    {
                        throw new RecorderNotFoundException(response.ErrorMessage ?? response.StatusDescription);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        var error = SimpleJson.DeserializeObject<RestError>(response.Content);
                        throw new ApplicationException(error.detail);
                    }
                    throw new ApplicationException(response.ErrorMessage ?? response.StatusDescription);
                }
                return response;
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                EventLogger.WriteEntry(ex);
                throw new ApplicationException("An unexpected error occured.");
            }
        }

        protected T Execute<T>(RestRequest request)
            where T: new()
        {
            try
            {
                var response = Execute(request);
                return DeserializeResponseContent<T>(response);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                EventLogger.WriteEntry(ex);
                throw new ApplicationException("An unexpected error occured.");
            }
        }

        protected T ExecuteResult<T>(RestRequest request)
        {
            var data = Execute<SimpleResult<T>>(request);
            return data.result;
        }

        protected static T DeserializeResponseContent<T>(IRestResponse response)
            where T : new()
        {
            string content = response.ContentLength == 0 ? String.Empty : response.Content;
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
}
