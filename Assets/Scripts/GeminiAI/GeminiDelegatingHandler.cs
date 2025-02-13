using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI
{
    public class GeminiDelegatingHandler : DelegatingHandler
    {
        private readonly GeminiOptions _geminiOptions;

        public GeminiDelegatingHandler(GeminiOptions geminiOptions)
        {
            InnerHandler = new HttpClientHandler();
            _geminiOptions = geminiOptions;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("x-goog-api-key", $"{_geminiOptions.ApiKey}");
            return base.SendAsync(request, cancellationToken);
        }
    }
}
