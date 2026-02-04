using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

public interface ISqlExecutor
{
    Task<int> ExecuteAsync(
        string sql,
        object param,
        IDbTransaction transaction);
}

