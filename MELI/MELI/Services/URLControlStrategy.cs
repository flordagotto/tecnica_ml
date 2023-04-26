using MELI.Services.Interfaces;
using System.Collections.Generic;

namespace MELI.Services
{
    public class URLControlStrategy : IControlStrategy
    {
        public void ContarCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar)
        {
            if (diccionarioAControlar.ContainsKey(parametroAControlar))
                diccionarioAControlar[parametroAControlar] = (diccionarioAControlar[parametroAControlar] + 1);
            else
                diccionarioAControlar.Add(parametroAControlar, 1);
        }

        public bool SuperaCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar, int valorMaximoPermitido)
            => (diccionarioAControlar.ContainsKey(parametroAControlar) && diccionarioAControlar[parametroAControlar] > valorMaximoPermitido);
    }
}
