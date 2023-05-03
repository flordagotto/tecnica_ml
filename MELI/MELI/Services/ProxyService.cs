using MELI.DataAccess.Interfaces;
using MELI.Services.Interfaces;
using System.Threading.Tasks;

namespace MELI.Services
{
    public class ProxyService : IProxyService
    {
        private readonly IRequestRepository _requestRepository;

        public ProxyService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task Get(string urlDestino)
        {
            
        }
    }
}
