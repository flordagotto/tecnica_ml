using System.Collections.Generic;

namespace MELI.Services.Interfaces
{
    public interface IControlDePeticionesService
    {
        void ContarCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar);
        bool SuperaCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar, int valorMaximo);
    }
}
