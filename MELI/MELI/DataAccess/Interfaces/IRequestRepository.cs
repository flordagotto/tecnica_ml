using MELI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MELI.DataAccess.Interfaces
{
    public interface IRequestRepository
    {
        Task<IEnumerable<Request>> GetAllRequests();
        Task<IEnumerable<Request>> GetRequestByParameter(Dictionary<string, string> parameterValueFilter);
        Task<int> CountRequestByParameter(Dictionary<string, string> parameterValueFilter);
        Task InsertRequest(Request request);
    }
}
