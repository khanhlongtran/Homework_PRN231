using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAuthentication
{
    public class SqlServerClientSourceAuthenticationHandler : IClientSourceAuthenticationHandler, IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection connection;

        public SqlServerClientSourceAuthenticationHandler(string connectionString)
        {
            _connectionString = connectionString;
            connection = new SqlConnection(_connectionString);
        }

        public void Dispose()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
            connection.Dispose();
        }

        public bool Validate(string clientSource)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            var query = "SELECT TOP 1 1 FROM ClientSources WHERE ClientId = @ClientSource AND GETDATE() >= ValidFrom AND GETDATE() <= ValidTo AND IsEnable = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ClientSource", clientSource);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }
            return false;
        }
    }

}
