using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data.Common;

namespace FasTnT.Infrastructure.Utils
{
    public class LoggerInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            Console.WriteLine(command.CommandText);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            Console.WriteLine(command.CommandText);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            Console.WriteLine(command.CommandText);
            return base.ScalarExecuting(command, eventData, result);
        }
    }
}
