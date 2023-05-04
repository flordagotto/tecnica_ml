using Dapper;
using MELI.DataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MELI.DataAccess
{
    public class ControlRepository //: IControlRepository
    {
        //protected readonly string _sqlConnectionString;
        //private readonly IConfiguration _configuration;

        //public ControlRepository(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //    _sqlConnectionString = _configuration.GetConnectionString("sqlConnection");
        //}

        //public async Task InsertOrUpdateAmountOfRequests(string table, string parameter, string value, DateTime date, int amount)
        //{
        //    var dateToString = date.ToString("yyyy-MM-dd");

        //    var query = $"IF EXISTS(SELECT 1 FROM {table} WHERE {parameter} = {value} AND date = '{dateToString}') " +
        //                    $"UPDATE {table} SET amount = {amount} WHERE {parameter} = {value} AND date = '{dateToString} " +
        //                $"ELSE " +
        //                $"INSERT INTO {table}({parameter}, date, amount) VALUES({value}, {dateToString}, {amount});";

        //    using var connection = new SqlConnection(_sqlConnectionString);
        //    var result = await connection.ExecuteAsync(query);
        //}

        //public async Task<int> GetAmountOfRequests(string table, string parameter, string value, DateTime date)
        //{
        //    var dateToString = date.ToString("yyyy-MM-dd");

        //    var query = $"SELECT amount FROM {table} WHERE {parameter} = '{value}' AND date = {dateToString}";

        //    using var connection = new SqlConnection(_sqlConnectionString);
        //    var result = await connection.QueryFirstOrDefaultAsync<int>(query);
        //    return result;
        //}
    }
}
