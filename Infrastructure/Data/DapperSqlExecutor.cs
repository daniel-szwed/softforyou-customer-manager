using Dapper;
using Domain.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class DapperSqlExecutor : ISqlExecutor
    {
        public Task<int> ExecuteAsync(string sql, object param, IDbTransaction transaction)
            => transaction.Connection.ExecuteAsync(sql, param, transaction);
    }
}
