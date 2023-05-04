using Dapper;
using MELI.DataAccess.Interfaces;
using MELI.Helpers;
using MELI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MELI.DataAccess
{
    public class RequestRepository : IRequestRepository
    {
        private readonly string _sqlConnectionString;
        private readonly IConfiguration _configuration;

        public RequestRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _sqlConnectionString = _configuration.GetConnectionString("sqlConnection");
        }

        public async Task<IEnumerable<Request>> GetAllRequests()
        {
            var query = "SELECT id, endpoint, url_request, date, response_code_proxy, response_code_api, source_ip, method " +
                "FROM request";

            using var connection = new SqlConnection(_sqlConnectionString);
            var result = await connection.QueryAsync<Request>(query);
            return result;
        }


        public async Task<IEnumerable<Request>> GetRequestByParameter(Dictionary<string, string> parameterValueFilter)
        {
            var query = "SELECT id, endpoint, url_request, date, response_code_proxy, response_code_api, source_ip, method " +
                "FROM request ";

            query = StringExtension.Where(query, parameterValueFilter);

            using var connection = new SqlConnection(_sqlConnectionString);
            var result = await connection.QueryAsync<Request>(query);
            return result;
        }

        public async Task<int> CountRequestByParameter(Dictionary<string, string> parameterValueFilter)
        {
            var query = "SELECT COUNT * " +
                "FROM request ";

            query = StringExtension.Where(query, parameterValueFilter);

            using var connection = new SqlConnection(_sqlConnectionString);
            var result = await connection.QueryFirstOrDefaultAsync<int>(query);
            return result;
        }

        public async Task InsertRequest(Request request)
        {
            var query = $"INSERT INTO request(endpoint, url_request, date, response_code_proxy, response_code_api, source_ip, method) VALUES ('{request.Endpoint}', '{request.UrlRequest}', '{request.Date.ToString("yyyy-MM-dd")}', {request.ResponseCodeProxy}, {request.ResponseCodeApi}, '{request.SourceIp}', '{request.Method}')";

            using var connection = new SqlConnection(_sqlConnectionString);
            var result = await connection.ExecuteAsync(query);
        }
    }
}
