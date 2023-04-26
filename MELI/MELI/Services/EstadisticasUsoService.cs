using MELI.Services.Interfaces;
using System.Collections.Generic;

namespace MELI.Services
{
    public class EstadisticasUsoService : IEstadisticasUsoService
    {
        public int GetCantidadPeticionesPorParametro(Dictionary<string, int> _accesos, string parametro) =>
             _accesos.ContainsKey(parametro) ? _accesos[parametro] : 0;

        public decimal GetPorcentajeSobreTotal(int cantidadPeticionesEnCondicion, int totalPeticiones)
        {
            decimal peticionesEnCondicionSegunRecibidas = (decimal)cantidadPeticionesEnCondicion / totalPeticiones;
            return peticionesEnCondicionSegunRecibidas*100;
        }
    }
}
