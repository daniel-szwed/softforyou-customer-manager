using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

public class DapperSqlExecutor : ISqlExecutor
{
    public Task<int> ExecuteAsync(string sql, object param, IDbTransaction transaction)
        => transaction.Connection.ExecuteAsync(sql, param, transaction);
}

