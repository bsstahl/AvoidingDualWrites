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
        const string _connectionKey = "sql";
        const string _servernameKey = "DBServerName";
        const string _dbNameKey = "DBDatabaseName";
        const string _dbUsernameKey = "DBUsername";
        const string _dbPasswordKey = "DBPassword";

        const string _selectQuery = "select top 1 [Id], [SendToAddress], [MessageSubject], [MessageBody] from dbo.[tblMessages] where [Sent]=0";
        const string _updateQuery = "update dbo.[tblMessages] set [Sent]=1 where [Id]=@id";

        private readonly string _connectionString;

        public MessageRepository(IConfiguration config)
        {
            _connectionString = string.Format(config.GetConnectionString(_connectionKey), config[_servernameKey], config[_dbNameKey], config[_dbUsernameKey], config[_dbPasswordKey]);
        }

        public Message GetUnsentMessage()
        {
            // TODO: Add error handling
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Message>(_selectQuery);
            }
        }

        public void UpdateMessageSent(Guid id)
        {
            // TODO: Add error handling and retry
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(_updateQuery, new { Id = id });
            }
        }
    }
}
