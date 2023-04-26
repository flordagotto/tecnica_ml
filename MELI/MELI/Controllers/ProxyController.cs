using MELI.Services;
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
        public IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public int Timeout { get; private set; }
        public string BaseUrl { get; private set; }
        private static Dictionary<string, int> _accesosIpOrigen = new Dictionary<string, int>();
        private static Dictionary<string, int> _accesosUrlDestino = new Dictionary<string, int>();
        private static int _cantidadPeticionesRecibidas = 0;
        private static int _cantidadPeticionesReenviadas = 0;
        private static int _cantidadPeticionesInvalidas = 0;
        private int _maxIp;
        private int _maxUrl;

        public MercadoLibreProxy(IConfiguration configuration)
        {
            Timeout = 100_000;
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPoliceErrors) => { return true; }
            });
            //this.VerificationServies = new Li>();
            BaseUrl = "https://api.mercadolibre.com";
            _configuration = configuration;
            _maxIp = Convert.ToInt32(_configuration.GetSection("MaxPeticionesPorIpOrigen").Value);
            //if maxIp > 0:
            //        this.VerificationServices.Add(IpVerifactor())
            _maxUrl = Convert.ToInt32(_configuration.GetSection("MaxPeticionesPorEndpointDestino").Value);
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(string urlDestino)
        {
            _cantidadPeticionesRecibidas++;
            //foreach (var verificationServies in collection)
            //{

            //}
            if (_maxIp > 0)
            {
                var ipOrigen = (HttpContext.Connection.RemoteIpAddress).ToString();
                ContarCantidadRequest(ipOrigen, _accesosIpOrigen);
                if(SuperaCantidadRequest(ipOrigen, _accesosUrlDestino, _maxIp))
                    return BadRequest("Se superaron la cantidad de request permitidas");
            }

            if (_maxUrl > 0)
            {
                ContarCantidadRequest(urlDestino, _accesosUrlDestino);
                if(SuperaCantidadRequest(urlDestino, _accesosUrlDestino, _maxUrl))
                    return BadRequest("Se superaron la cantidad de request permitidas");
            }

            var urlBuilder = BaseUrl + "/" + urlDestino;

            var client = _httpClient;
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.RequestUri = new Uri(urlBuilder.ToString());

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
            return Ok(  $"La cantidad de peticiones realizadas por la ip de origen {ipOrigen} fue {cantidadPorIp}. " +
                        $"Representa el {requestPorIpSegunTotal*100}% del total de peticiones recibidas");
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
            return Ok(  $"La cantidad de peticiones reenviadas a la api de mercado libre correctamente fue {_cantidadPeticionesReenviadas}. " +
                        $"Representa el {peticionesReenviadasSegunRecibidas*100}% del total de peticiones recibidas");
        }

        [HttpGet]
        [Route("EstadisticaUsoInvalido")]
        public IActionResult GetEstadisticaUsoPeticionesInvalidasSegunPeticionesRecibidas()
        {
            decimal peticionesInvalidasSegunRecibidas = EstadisticasUsoService.GetEstadisticaUsoPeticionesEnCondicionSegunPeticionesRecibidas(_cantidadPeticionesInvalidas, _cantidadPeticionesRecibidas);
            return Ok($"La cantidad de peticiones reenviadas a la api de mercado libre correctamente fue {_cantidadPeticionesInvalidas}. " +
                        $"Representa el {peticionesInvalidasSegunRecibidas * 100}% del total de peticiones recibidas");
        }


        private void ContarCantidadRequest(string parametroAControlar, Dictionary<string, int> listaAControlar)
        {
            if (listaAControlar.ContainsKey(parametroAControlar))
                listaAControlar[parametroAControlar] = (listaAControlar[parametroAControlar] + 1);
            else
                listaAControlar.Add(parametroAControlar, 1);
        }

        private bool SuperaCantidadRequest(string parametroAControlar, Dictionary<string, int> listaAControlar, int valorMaximo)
        {
            if (listaAControlar.ContainsKey(parametroAControlar) && listaAControlar[parametroAControlar] > valorMaximo)
                return true;
            return false;
        }
    }
}
