using MELI.Services;
using MELI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MELI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MercadoLibreProxy : ControllerBase
    {
        public readonly int Timeout = 10000;
        public readonly string BaseUrl = "https://api.mercadolibre.com";
        private readonly int _maximaCantidadRequestPorIpOrigen;
        private readonly int _maximaCantidadRequestPorEndpoint;
        private readonly bool _controlarPorIp;
        private readonly bool _controlarPorEndpoint;

        public readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IControlDePeticionesService _controlService;
        private readonly IEstadisticasUsoService _estadisticasService;

        private static Dictionary<string, int> _accesosIpOrigen = new Dictionary<string, int>();
        private static Dictionary<string, int> _accesosUrlDestino = new Dictionary<string, int>();
        private static int _cantidadPeticionesRecibidas = 0;
        private static int _cantidadPeticionesReenviadas = 0;
        private static int _cantidadPeticionesInvalidas = 0;

        public MercadoLibreProxy(IConfiguration configuration, IControlDePeticionesService controlService, IEstadisticasUsoService estadisticasService)
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPoliceErrors) => { return true; }
            });
            _configuration = configuration;
            _controlService = controlService;
            _estadisticasService = estadisticasService;
            _maximaCantidadRequestPorIpOrigen = Convert.ToInt32(_configuration.GetSection("MaxPeticionesPorIpOrigen").Value);
            _maximaCantidadRequestPorEndpoint = Convert.ToInt32(_configuration.GetSection("MaxPeticionesPorEndpointDestino").Value);
            _controlarPorIp = _maximaCantidadRequestPorIpOrigen > 0;
            _controlarPorEndpoint = _maximaCantidadRequestPorEndpoint > 0;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(string urlDestino)
        {
            _cantidadPeticionesRecibidas++;
            var ipOrigen = (HttpContext.Connection.RemoteIpAddress).ToString();
            var endpoint = urlDestino.Split('/')[0];

            _controlService.ContarCantidadRequest(ipOrigen, _accesosIpOrigen);
            _controlService.ContarCantidadRequest(endpoint, _accesosUrlDestino);

            if (_controlarPorIp && _controlService.SuperaCantidadRequest(ipOrigen, _accesosIpOrigen, _maximaCantidadRequestPorIpOrigen))
            {
                return BadRequest("Se superó la cantidad de peticiones por ip de origen");
            }
            if (_controlarPorEndpoint && _controlService.SuperaCantidadRequest(endpoint, _accesosUrlDestino, _maximaCantidadRequestPorEndpoint))
            {
                return BadRequest($"Se superó la cantidad de peticiones al endpoint {BaseUrl}/{endpoint}");
            }

            var urlEndpoint = $"{BaseUrl}/{urlDestino}";

            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.RequestUri = new Uri(urlEndpoint.ToString());

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);

                try
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        _cantidadPeticionesInvalidas++;
                        return BadRequest("Hubo un problema: verifique la URL enviada");
                    }

                    var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _cantidadPeticionesReenviadas++;
                    return Ok(responseText);
                }
                finally
                {
                    response.Dispose();
                }
            }
        }

        [HttpGet]
        [Route("EstadisticaUsoPorIp")]
        public IActionResult GetEstadisticaUsoPorIpSegunPeticionesRecibidas(string ipOrigen)
        {
            int cantidadPorIp = _estadisticasService.GetCantidadPeticionesPorParametro(_accesosIpOrigen, ipOrigen);
            decimal porcentajeRequestPorIpSegunTotal = _estadisticasService.GetPorcentajeSobreTotal(cantidadPorIp, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones realizadas por la ip de origen {ipOrigen} fue {cantidadPorIp}. " +
                        $"Representa el {porcentajeRequestPorIpSegunTotal}% del total de peticiones recibidas");
        }

        [HttpGet]
        [Route("EstadisticaUsoPorEndpoint")]
        public IActionResult GetEstadisticaUsoPorUrlDestinoSegunPeticionesRecibidas(string urlDestino)
        {
            int cantidadPorEndpoint = _estadisticasService.GetCantidadPeticionesPorParametro(_accesosUrlDestino, urlDestino);
            decimal porcentajeRequestPorEndpointSegunTotal = _estadisticasService.GetPorcentajeSobreTotal(cantidadPorEndpoint, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones realizadas al endpoint destino {BaseUrl}/{urlDestino} fue {cantidadPorEndpoint}. " +
                        $"Representa el {porcentajeRequestPorEndpointSegunTotal}% del total de peticiones recibidas");
        }

        [HttpGet]
        [Route("EstadisticaUsoTotal")]
        public IActionResult GetEstadisticaPeticionesRecibidas()
        {
            return Ok($"La cantidad de peticiones realizadas fue {_cantidadPeticionesRecibidas}");
        }

        [HttpGet]
        [Route("EstadisticaUsoCorrecto")]
        public IActionResult GetEstadisticaUsoPeticionesReenviadasSegunPeticionesRecibidas()
        {
            decimal porcentajePeticionesReenviadasSegunRecibidas = _estadisticasService.GetPorcentajeSobreTotal(_cantidadPeticionesReenviadas, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones reenviadas a la api de mercado libre correctamente fue {_cantidadPeticionesReenviadas}. " +
                        $"Representa el {porcentajePeticionesReenviadasSegunRecibidas}% del total de peticiones recibidas");
        }

        [HttpGet]
        [Route("EstadisticaUsoInvalido")]
        public IActionResult GetEstadisticaUsoPeticionesInvalidasSegunPeticionesRecibidas()
        {
            decimal porcentajePeticionesInvalidasSegunRecibidas = _estadisticasService.GetPorcentajeSobreTotal(_cantidadPeticionesInvalidas, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones reenviadas a la api de mercado libre correctamente fue {_cantidadPeticionesInvalidas}. " +
                        $"Representa el {porcentajePeticionesInvalidasSegunRecibidas}% del total de peticiones recibidas");
        }
    }
}
