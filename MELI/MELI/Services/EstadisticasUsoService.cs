using System.Collections.Generic;

namespace MELI.Services
{
    public static class EstadisticasUsoService
    {
        public static int GetCantidadPeticionesPorParametro(Dictionary<string, int> _accesos, string parametro) =>
             _accesos.ContainsKey(parametro) ? _accesos[parametro] : 0;

        public static decimal GetEstadisticaUsoPorParametroSegunPeticionesRecibidas(int cantidadPorParametro, int totalPeticiones)
        {
            decimal requestPorParametro = (decimal)cantidadPorParametro / totalPeticiones;
            return requestPorParametro;
        }

        public static decimal GetEstadisticaUsoPeticionesEnCondicionSegunPeticionesRecibidas(int cantidadPeticionesEnCondicion, int totalPeticiones)
        {
            decimal peticionesEnCondicionSegunRecibidas = (decimal)cantidadPeticionesEnCondicion / totalPeticiones;
            return peticionesEnCondicionSegunRecibidas;
        }

        public static decimal GetPorcentaje(decimal value)
        {
            return value * 100;
        }
    }
}
