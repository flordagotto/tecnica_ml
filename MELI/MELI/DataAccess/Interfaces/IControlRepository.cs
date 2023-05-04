using MELI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MELI.DataAccess.Interfaces
{
    public interface IControlRepository<T>
    {
        Task InsertOrUpdateAmountOfRequests(string value, DateTime date, int amount);
        Task<int> GetAmountOfRequests(string value, DateTime date);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetByParameter(Dictionary<string, string> parameterValueFilter);
    }
}
