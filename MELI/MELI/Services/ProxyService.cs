using MELI.DataAccess.Interfaces;
using MELI.Models;
using MELI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MELI.Services
{
    public class ProxyService : IProxyService
    {
        private readonly IRequestRepository _requestRepository;
        //private readonly List<IControlRepository> _controlList;
        private readonly HttpClient _httpClient;
        public readonly string BaseUrl = "https://api.mercadolibre.com";

        public ProxyService(IRequestRepository requestRepository)
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPoliceErrors) => { return true; }
            });
            _requestRepository = requestRepository;

        }

        public async Task<IActionResult> Get(string urlRequest, string sourceIp)
        {
            string endpoint = urlRequest.Split('/')[0];

            var urlEndpoint = $"{BaseUrl}/{urlRequest}";

            using (var currentRequest = new HttpRequestMessage())
            {
                currentRequest.Method = new HttpMethod("GET");
                currentRequest.RequestUri = new Uri(urlEndpoint.ToString());

                var response = await _httpClient.SendAsync(currentRequest, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);
                var statusCodeApi = (int)response.StatusCode;

                var request = new Request
                {
                    Endpoint = endpoint,
                    UrlRequest = urlRequest,
                    Date = DateTime.Today,
                    ResponseCodeApi = statusCodeApi,
                    ResponseCodeProxy = statusCodeApi,
                    SourceIp = sourceIp,
                    Method = "GET"
                };
                await _requestRepository.InsertRequest(request);

                return new JsonResult(response);
            }


        }
    }
}
