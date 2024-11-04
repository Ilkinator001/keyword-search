using KeywordSearchService.Logic;
using KeywordSearchService.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeywordSearchService
{
    public partial class KeywordSearchService : ServiceBase
    {
        private HttpListener _listener;
        private readonly FileSearchService _fileSearchService;

        public KeywordSearchService()
        {
            InitializeComponent();
            _fileSearchService = new FileSearchService();
        }

        protected override void OnStart(string[] args)
        {
            // HTTP-Listener starten
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:5000/api/");
            _listener.Start();
            _listener.BeginGetContext(OnRequestReceived, null);
        }

        protected override void OnStop()
        {
            _listener.Stop();
            _listener.Close();
        }

        private void OnRequestReceived(IAsyncResult result)
        {
            if (!_listener.IsListening)
                return;

            var context = _listener.EndGetContext(result);
            _listener.BeginGetContext(OnRequestReceived, null); // Nächste Anfrage akzeptieren

            // Verarbeiten der Anfrage
            if (context.Request.HttpMethod == "POST" && context.Request.Url.AbsolutePath == "/api/search")
            {
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    var json = reader.ReadToEnd();
                    var request = JsonConvert.DeserializeObject<SearchRequest>(json);

                    if (string.IsNullOrWhiteSpace(request.Directory) || string.IsNullOrWhiteSpace(request.Keyword))
                    {
                        SendResponse(context, 400, "Directory and keyword are required.");
                    }
                    else
                    {
                        var results = _fileSearchService.SearchFiles(request.Directory, request.Keyword);
                        SendResponse(context, 200, JsonConvert.SerializeObject(results));
                    }
                }
            }
            else
            {
                SendResponse(context, 404, "Endpoint not found.");
            }
        }

        private void SendResponse(HttpListenerContext context, int statusCode, string responseString)
        {
            context.Response.StatusCode = statusCode;
            var buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}
