using System;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.TinyIoc;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    public class RestBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register(typeof(INancyModule), typeof(TvServerRecorderModule), typeof(TvServerRecorderModule).FullName).AsSingleton();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            Nancy.Security.Csrf.Disable(pipelines);
            Nancy.Json.JsonSettings.MaxJsonLength = int.MaxValue;
            Nancy.Json.JsonSettings.ISO8601DateFormat = false;
            Nancy.Json.JsonSettings.RetainCasing = true;

            pipelines.OnError.AddItemToStartOfPipeline(ReturnJsonError);
        }

        private static Response ReturnJsonError(NancyContext context, Exception ex)
        {
            Response response = null;

            if (context.Request.Headers.Accept.Any(a => a.Item1.IndexOf("application/json", StringComparison.InvariantCultureIgnoreCase) >= 0))
            {
                var serializer = new Nancy.Json.JavaScriptSerializer();
                response = new Response()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "application/json",
                    Contents = s =>
                    {
                        StreamWriter writer = new StreamWriter(s, new UTF8Encoding(false)) { AutoFlush = true }; // Don't dispose this!
                        writer.Write(serializer.Serialize(new { detail = ex.Message }));
                    }
                };
            }

            try
            {
                StringBuilder log = GetLogForRequest(context);
                log.AppendLine().AppendLine("error: ").Append(ex.ToString());
                TvLibrary.Log.Log.Error(log.ToString());
            }
            catch
            {
            }

            return response;
        }

        private static StringBuilder GetLogForRequest(NancyContext context)
        {
            StringBuilder log = null;
            string path = context.Request.Url.Path.ToLowerInvariant();
            if (!path.EndsWith("/help.css") && !path.EndsWith("/help"))
            {
                log = new StringBuilder();
                log.Append("REST ").Append(context.Request.Method).Append(" ").Append(context.Request.Url.Path).Append(context.Request.Url.Query);
                if (context.Response != null)
                {
                    log.AppendLine().Append("status: ").Append(context.Response.StatusCode.ToString());
                }
                if (context.Request.Method == "POST")
                {
                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                    {
                        string json = reader.ReadToEnd().Trim();
                        if (!String.IsNullOrEmpty(json))
                        {
                            log.AppendLine().Append("input (").Append(context.Request.Headers.ContentType).Append("): ").Append(json);
                        }
                    }
                }
            }
            return log;
        }
    }
}
