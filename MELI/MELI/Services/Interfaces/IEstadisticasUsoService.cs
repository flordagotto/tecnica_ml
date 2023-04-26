using System.Collections.Generic;

namespace MELI.Services.Interfaces
{
    public interface IEstadisticasUsoService
    {
        int GetCantidadPeticionesPorParametro(Dictionary<string, int> _accesos, string parametro);
        decimal GetPorcentajeSobreTotal(int cantidadPorParametro, int totalPeticiones);
    }
}
