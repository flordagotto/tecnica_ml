using System.Collections.Generic;
using MELI.Services.Interfaces;

namespace MELI.Services
{
    public class ControlDePeticionesService : IControlDePeticionesService
    {
        public void ContarCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar)
        {
            if (diccionarioAControlar.ContainsKey(parametroAControlar))
                diccionarioAControlar[parametroAControlar] = diccionarioAControlar[parametroAControlar] + 1;
            else
                diccionarioAControlar.Add(parametroAControlar, 1);
        }
        public bool SuperaCantidadRequest(string parametroAControlar, Dictionary<string, int> diccionarioAControlar, int valorMaximo)
            => (diccionarioAControlar.ContainsKey(parametroAControlar) && diccionarioAControlar[parametroAControlar] > valorMaximo);
    }
}
