using System;
using System.Collections.Generic;
using RestSharp;
using ArgusTV.DataContracts;

namespace ArgusTV.ServiceProxy
{
    public abstract class RestProxyBase
    {
        protected RestClient _client;

        public RestProxyBase(string module)
        {
            var url = (ProxyFactory.ServerSettings.Transport == ServiceTransport.Https ? "https://" : "http://")
                + ProxyFactory.ServerSettings.ServerName + ":" + ProxyFactory.ServerSettings.Port
                + "/ArgusTV/" + module;
            _client = new RestClient(url);
            if (ProxyFactory.ServerSettings.Transport == ServiceTransport.Https)
            {
                _client.Authenticator = new HttpBasicAuthenticator(ProxyFactory.ServerSettings.UserName, ProxyFactory.ServerSettings.Password);
            }
        }

        private class SchedulerJsonSerializer : RestSharp.Serializers.ISerializer
        {
            public SchedulerJsonSerializer()
            {
                ContentType = "application/json";
            }

            public string Serialize(object obj)
            {
                return SimpleJson.SerializeObject(obj, new SchedulerJsonSerializerStrategy());
            }

            public string DateFormat { get; set; }
            public string RootElement { get; set; }
            public string Namespace { get; set; }
            public string ContentType { get; set; }
        }

        protected class SchedulerJsonSerializerStrategy : PocoJsonSerializerStrategy
        {
            private static readonly long _initialJavaScriptDateTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            private static readonly DateTime _minimumJavaScriptDate = new DateTime(100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            public override object DeserializeObject(object value, Type type)
            {
                bool isString = value is string;

                Type valueType = null;
                bool isEnum = type.IsEnum || (IsNullable(type, out valueType) && valueType.IsEnum);
                if (isEnum && (isString || value is Int32 || value is Int64))
                {
                    if (!isString
                        || Enum.IsDefined(valueType ?? type, value))
                    {
                        return Enum.Parse(valueType ?? type, value.ToString());
                    }
                }
                else if (type == typeof(DateTime)
                    || type == typeof(DateTime?))
                {
                    var s = value as string;
                    if (s != null
                        && s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal))
                    {
                        int tzCharIndex = s.IndexOfAny(new char[] { '+', '-' }, 7);
                        long javaScriptTicks = Convert.ToInt64(s.Substring(6, (tzCharIndex > 0) ? tzCharIndex - 6 : s.Length - 8));
                        DateTime time = new DateTime((javaScriptTicks * 10000) + _initialJavaScriptDateTicks, DateTimeKind.Utc);
                        if (tzCharIndex > 0)
                        {
                            time = time.ToLocalTime();
                        }
                        return time;
                    }
                }
                else if (type == typeof(ServiceEvent))
                {
                    var jsonObject = (JsonObject)value;
                    var @event = new ServiceEvent()
                    {
                        Name = (string)jsonObject["Name"],
                        Time = (DateTime)DeserializeObject(jsonObject["Time"], typeof(DateTime))
                    };
                    var args = (JsonArray)jsonObject["Arguments"];
                    if (args != null)
                    {
                        List<object> arguments = new List<object>();
                        switch (@event.Name)
                        {
                            case DataContracts.ServiceEventNames.ConfigurationChanged:
                                arguments.Add(DeserializeObject(args[0], typeof(string)));
                                arguments.Add(DeserializeObject(args[1], typeof(string)));
                                break;

                            case DataContracts.ServiceEventNames.ScheduleChanged:
                                arguments.Add(DeserializeObject(args[0], typeof(Guid)));
                                arguments.Add(DeserializeObject(args[1], typeof(int)));
                                break;

                            case DataContracts.ServiceEventNames.RecordingStarted:
                            case DataContracts.ServiceEventNames.RecordingEnded:
                                arguments.Add(DeserializeObject(args[0], typeof(Recording)));
                                break;

                            case DataContracts.ServiceEventNames.LiveStreamStarted:
                            case DataContracts.ServiceEventNames.LiveStreamTuned:
                            case DataContracts.ServiceEventNames.LiveStreamEnded:
                            case DataContracts.ServiceEventNames.LiveStreamAborted:
                                arguments.Add(DeserializeObject(args[0], typeof(LiveStream)));
                                if (@event.Name == DataContracts.ServiceEventNames.LiveStreamAborted)
                                {
                                    arguments.Add(DeserializeObject(args[1], typeof(LiveStreamAbortReason)));
                                    arguments.Add(DeserializeObject(args[2], typeof(UpcomingProgram)));
                                }
                                break;

                            default:
                                foreach (JsonObject arg in args)
                                {
                                    arguments.Add(arg);
                                }
                                break;
                        }
                        @event.Arguments = arguments.ToArray();
                    }
                    return @event;
                }
                return base.DeserializeObject(value, type);
            }

            protected override bool TrySerializeKnownTypes(object input, out object output)
            {
                if (input is DateTime)
                {
                    DateTime value = (DateTime)input;
                    DateTime time = value.ToUniversalTime();

                    string suffix = "";
                    if (value.Kind != DateTimeKind.Utc)
                    {
                        TimeSpan localTZOffset;
                        if (value >= time)
                        {
                            localTZOffset = value - time;
                            suffix = "+";
                        }
                        else
                        {
                            localTZOffset = time - value;
                            suffix = "-";
                        }
                        suffix += localTZOffset.ToString("hhmm");
                    }

                    if (time < _minimumJavaScriptDate)
                    {
                        time = _minimumJavaScriptDate;
                    }
                    long ticks = (time.Ticks - _initialJavaScriptDateTicks) / (long)10000;
                    output = "/Date(" + ticks + suffix + ")/";
                    return true;
                }
                return base.TrySerializeKnownTypes(input, out output);
            }

            private static bool IsNullable(Type type, out Type valueType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    valueType = type.GetGenericArguments()[0];
                    return true;
                }

                valueType = null;
                return false;
            }
        }

        protected RestRequest NewRequest(string url, Method method)
        {
            return new RestRequest(url, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new SchedulerJsonSerializer()
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
                            //Logger.Error(error.detail);
                        }
                        else
                        {
                            //Logger.Error(r.ErrorMessage ?? r.StatusDescription);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                //Logger.Error(ex.ToString());
                //EventLogger.WriteEntry(ex);
                throw new ApplicationException("An unexpected error occured.");
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

        protected IRestResponse Execute(RestRequest request, bool logError = true)
        {
            try
            {
                var response = _client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == 0 && IsConnectionError(response.ErrorException))
                    {
                        throw new ArgusTVNotFoundException(response.ErrorMessage ?? response.StatusDescription);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        var error = SimpleJson.DeserializeObject<RestError>(response.Content);
                        throw new ArgusTVException(error.detail);
                    }
                    throw new ArgusTVException(response.ErrorMessage ?? response.StatusDescription);
                }
                return response;
            }
            catch (ArgusTVException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (logError)
                {
                    //Logger.Error(ex.ToString());
                    //EventLogger.WriteEntry(ex);
                }
                throw new ArgusTVUnexpectedErrorException("An unexpected error occured.");
            }
        }

        protected T Execute<T>(RestRequest request, bool logError = true)
            where T : new()
        {
            try
            {
                var response = Execute(request, logError);
                return DeserializeResponseContent<T>(response);
            }
            catch (ArgusTVException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (logError)
                {
                    //Logger.Error(ex.ToString());
                    //EventLogger.WriteEntry(ex);
                }
                throw new ArgusTVUnexpectedErrorException("An unexpected error occured.");
            }
        }

        protected static T DeserializeResponseContent<T>(IRestResponse response)
            where T : new()
        {
            T result;
            if (response.ContentLength == 0)
            {
                result = default(T);
            }
            else
            {
                result = SimpleJson.DeserializeObject<T>(response.Content, new SchedulerJsonSerializerStrategy());
            }
            return result;
        }

        protected string ToIso8601(DateTime time)
        {
            if (time.Kind == DateTimeKind.Unspecified)
            {
                return new DateTime(time.Ticks, DateTimeKind.Local).ToString("o");
            }
            return time.ToString("o");
        }

        private class RestError
        {
            public string detail { get; set; }
        }
    }
}
