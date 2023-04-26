using MELI.Services.Interfaces;
using System.Collections.Generic;

namespace MELI.Services
{
    public class URLControlStrategy : IControlStrategy
    {
        public readonly int ValorMaximo;

        public URLControlStrategy (int valorMaximo)
        {
            ValorMaximo = valorMaximo;
        }

        public void ContarCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar)
        {
            if (diccionarioAControlar.ContainsKey(parametroAControlar))
                diccionarioAControlar[parametroAControlar] = (diccionarioAControlar[parametroAControlar] + 1);
            else
                diccionarioAControlar.Add(parametroAControlar, 1);
        }

        public bool SuperaCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar)
            => (diccionarioAControlar.ContainsKey(parametroAControlar) && diccionarioAControlar[parametroAControlar] > ValorMaximo);
    }
}
