using Dapper;
using MELI.DataAccess.Interfaces;
using MELI.Helpers;
using MELI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MELI.DataAccess
{
    public class EndpointControlRepository : IControlRepository<EndpointControl>
    {
        public const string TABLENAME = "endpoint_control";
        private readonly string _sqlConnectionString;
        private readonly IConfiguration _configuration;

        public EndpointControlRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _sqlConnectionString = _configuration.GetConnectionString("sqlConnection");
        }

        public async Task<IEnumerable<EndpointControl>> GetAll()
        {
            var query = "SELECT endpoint, amount, date " +
                $"FROM {TABLENAME}";

            using var connection = new SqlConnection(_sqlConnectionString);
            var result = await connection.QueryAsync<EndpointControl>(query);
            return result;
        }


        public async Task<IEnumerable<EndpointControl>> GetByParameter(Dictionary<string, string> parameterValueFilter)
        {
            var query = "SELECT endpoint, amount, date " +
                $"FROM {TABLENAME}";

            query = StringExtension.Where(query, parameterValueFilter);

            using var connection = new SqlConnection(_sqlConnectionString);
            var result = await connection.QueryAsync<EndpointControl>(query);
            return result;
        }

        public async Task InsertOrUpdateAmountOfRequests(string value, DateTime date, int amount)
        {
            var dateToString = date.ToString("yyyy-MM-dd");

            var query = $"IF EXISTS(SELECT 1 FROM {TABLENAME} WHERE endpoint = {value} AND date = '{dateToString}') " +
                            $"UPDATE {TABLENAME} SET amount = {amount} WHERE endpoint = {value} AND date = '{dateToString} " +
                        $"ELSE " +
                        $"INSERT INTO {TABLENAME}(endpoint, date, amount) VALUES({value}, {dateToString}, {amount});";

            using var connection = new SqlConnection(_sqlConnectionString);
            var result = await connection.ExecuteAsync(query);
        }

        public async Task<int> GetAmountOfRequests(string value, DateTime date)
        {
            var dateToString = date.ToString("yyyy-MM-dd");

            var query = $"SELECT amount FROM {TABLENAME} WHERE endpoint = '{value}' AND date = {dateToString}";

            using var connection = new SqlConnection(_sqlConnectionString);
            var result = await connection.QueryFirstOrDefaultAsync<int>(query);
            return result;
        }
    }
}
