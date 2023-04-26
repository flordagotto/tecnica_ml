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

        public readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private static List<IControlStrategy> controlStrategies = new List<IControlStrategy>();
        private static Dictionary<string, int> _accesosIpOrigen = new Dictionary<string, int>();
        private static Dictionary<string, int> _accesosUrlDestino = new Dictionary<string, int>();
        private static int _cantidadPeticionesRecibidas = 0;
        private static int _cantidadPeticionesReenviadas = 0;
        private static int _cantidadPeticionesInvalidas = 0;
        private int _maximaCantidadRequestPorIpOrigen;
        private int _maximaCantidadRequestPorEndpoint;

        public MercadoLibreProxy(IConfiguration configuration)
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPoliceErrors) => { return true; }
            });
            _configuration = configuration;

            _maximaCantidadRequestPorIpOrigen = Convert.ToInt32(_configuration.GetSection("MaxPeticionesPorIpOrigen").Value);
            _maximaCantidadRequestPorEndpoint = Convert.ToInt32(_configuration.GetSection("MaxPeticionesPorEndpointDestino").Value);

            if (_maximaCantidadRequestPorIpOrigen > 0)
            {
                controlStrategies.Add(new IpControlStrategy());
            }

            if (_maximaCantidadRequestPorEndpoint > 0)
            {
                controlStrategies.Add(new URLControlStrategy());
            }
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(string urlDestino)
        {
            _cantidadPeticionesRecibidas++;
            var ipOrigen = (HttpContext.Connection.RemoteIpAddress).ToString();

            foreach (var controlStrategy in controlStrategies)
            {
                controlStrategy.ContarCantidadRequest();
            }

            if (_maximaCantidadRequestPorIpOrigen > 0)
            {
                HttpContext.Connection.
                ContarCantidadRequest(ipOrigen, _accesosIpOrigen);
                if (SuperaCantidadRequest(ipOrigen, _accesosUrlDestino, _maximaCantidadRequestPorIpOrigen))
                    return BadRequest("Se superaron la cantidad de request permitidas");
            }

            if (_maximaCantidadRequestPorEndpoint > 0)
            {
                ContarCantidadRequest(urlDestino, _accesosUrlDestino);
                if (SuperaCantidadRequest(urlDestino, _accesosUrlDestino, _maximaCantidadRequestPorEndpoint))
                    return BadRequest("Se superaron la cantidad de request permitidas");
            }

            var urlEndpoint = BaseUrl + "/" + urlDestino;

            var client = _httpClient;
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.RequestUri = new Uri(urlEndpoint.ToString());

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);
                try
                {
                    if (!response.IsSuccessStatusCode)
                        return BadRequest("Hubo un problema: verifique la URL enviada");

                    var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _cantidadPeticionesReenviadas++;
                    return Ok(responseText);
                }
                catch (Exception ex)
                {
                    _cantidadPeticionesInvalidas++;
                    return BadRequest(ex);
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
            int cantidadPorIp = EstadisticasUsoService.GetCantidadPeticionesPorParametro(_accesosIpOrigen, ipOrigen);
            decimal requestPorIpSegunTotal = EstadisticasUsoService.GetEstadisticaUsoPorParametroSegunPeticionesRecibidas(cantidadPorIp, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones realizadas por la ip de origen {ipOrigen} fue {cantidadPorIp}. " +
                        $"Representa el {requestPorIpSegunTotal * 100}% del total de peticiones recibidas");
        }

        [HttpGet]
        [Route("EstadisticaUsoPorEndpoint")]
        public IActionResult GetEstadisticaUsoPorUrlDestinoSegunPeticionesRecibidas(string urlDestino)
        {
            int cantidadPorEndpoint = EstadisticasUsoService.GetCantidadPeticionesPorParametro(_accesosUrlDestino, urlDestino);
            decimal requestPorEndpointSegunTotal = EstadisticasUsoService.GetEstadisticaUsoPorParametroSegunPeticionesRecibidas(cantidadPorEndpoint, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones realizadas al endpoint destino {BaseUrl}/{urlDestino} fue {cantidadPorEndpoint}. " +
                        $"Representa el {requestPorEndpointSegunTotal * 100}% del total de peticiones recibidas");
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
            decimal peticionesReenviadasSegunRecibidas = EstadisticasUsoService.GetEstadisticaUsoPeticionesEnCondicionSegunPeticionesRecibidas(_cantidadPeticionesReenviadas, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones reenviadas a la api de mercado libre correctamente fue {_cantidadPeticionesReenviadas}. " +
                        $"Representa el {peticionesReenviadasSegunRecibidas * 100}% del total de peticiones recibidas");
        }

        [HttpGet]
        [Route("EstadisticaUsoInvalido")]
        public IActionResult GetEstadisticaUsoPeticionesInvalidasSegunPeticionesRecibidas()
        {
            decimal peticionesInvalidasSegunRecibidas = EstadisticasUsoService.GetEstadisticaUsoPeticionesEnCondicionSegunPeticionesRecibidas(_cantidadPeticionesInvalidas, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones reenviadas a la api de mercado libre correctamente fue {_cantidadPeticionesInvalidas}. " +
                        $"Representa el {peticionesInvalidasSegunRecibidas * 100}% del total de peticiones recibidas");
        }
    }
}
