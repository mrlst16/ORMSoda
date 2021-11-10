using ORMSoda.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ORMSoda
{
    public abstract class SqlSprocBase<TRequest, TResponse> : ISprocCall<TRequest, TResponse>
    {

        protected readonly string _connectionString;
        public SqlSprocBase()
        {
        }

        protected SqlSprocBase(
            string connectionString
            )
        {
            _connectionString = connectionString;
        }

        public abstract Task<TResponse> Call(TRequest request);

        protected abstract Task<TResponse> Map(DataSet request);
        protected virtual bool TryCallingSproc(DataSet dataSet, string sprocName, SqlParameter[] parameters)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"exec {sprocName}";
                command.Parameters.AddRange(parameters);

                using (var adapter = new SqlDataAdapter(command))
                {
                    result = adapter.Fill(dataSet) == 1;
                }
            }
            return result;
        }

        protected bool TryGetValue<T>(DataRow row, string columnName, out T result)
        {
            result = default(T);
            try
            {
                result = (T)row[columnName];
            }
            catch (Exception e)
            {
                //Just catching for now
                return false;
            }
            return false;
        }

        protected T GetValue<T>(DataRow row, string columnName, T defaultValue = default(T))
        {
            var result = default(T);
            try
            {
                result = (T)row[columnName];
            }
            catch (Exception e)
            {
                //Just catching for now
                result = defaultValue;
            }
            return result;
        }
    }


}
