using System.Collections.Generic;

namespace MELI.Services.Interfaces
{
    public interface IControlStrategy
    {
        void ContarCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar);
        bool SuperaCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar, int valorMaximoPermitido);
    }
}
