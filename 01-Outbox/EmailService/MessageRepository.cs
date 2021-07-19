using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace EmailService
{
    public class MessageRepository : IMessageRepository
    {
        const string _selectQuery = "select top 1 Id, SendToAddress, MessageSubject, MessageBody from dbo.tblMessages where Sent=0";
        const string _updateQuery = "update dbo.tblMessages set Sent=1 where Id=@id";
        const string _connection = "Server=tcp:{0}.database.windows.net,1433;Initial Catalog={1};Persist Security Info=False;User ID={2};Password={3};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly string _connectionString;

        public MessageRepository(IConfiguration config)
        {
            _connectionString = string.Format(_connection, config["DBServerName"], config["DBDatabaseName"], config["DBUsername"], config["DBPassword"]);
        }

        public Message GetUnsentMessage()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Message>(_selectQuery);
            }
        }

        public void UpdateMessageSent(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(_updateQuery, new { Id = id });
            }
        }
    }
}
