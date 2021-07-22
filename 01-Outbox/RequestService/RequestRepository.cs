using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace RequestService
{
    public class RequestRepository : IRequestRepository
    {
        const string _connectionKey = "sql";
        const string _servernameKey = "DBServerName";
        const string _dbNameKey = "DBDatabaseName";
        const string _dbUsernameKey = "DBUsername";
        const string _dbPasswordKey = "DBPassword";

        const string _insertRequestSql = "insert into tblRequests (Id, CustomerEmail, Description) values (@id, @customerEmail, @description)";
        const string _insertMessageSql = "insert into tblMessages (Id, RequestId, SendToAddress, MessageSubject, MessageBody, Sent) values (@id, @requestId, @sendToAddress, @messageSubject, @messageBody, @sent)";

        private readonly string _connectionString;

        public RequestRepository(IConfiguration config)
        {
            _connectionString = string.Format(config.GetConnectionString(_connectionKey), config[_servernameKey], config[_dbNameKey], config[_dbUsernameKey], config[_dbPasswordKey]);
        }

        public void SaveRequest(Guid id, string customerEmail, string description, string emailSubject, string emailBody)
        {
            Log.Information("Begin SaveRequest for {id}", id);

            // TODO: Retry multiple times if the process fails

            try
            {

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var trx = connection.BeginTransaction())
                    {
                        // Update tblRequests
                        var requestData = new
                        {
                            id = id,
                            customerEmail = customerEmail,
                            description = description
                        };
                        var requestRowsAdded = connection.Execute(_insertRequestSql, requestData, transaction: trx);

                        // Update tblMessages
                        var messageData = new
                        {
                            id = Guid.NewGuid(),
                            requestId = id,
                            sendToAddress = customerEmail,
                            messageSubject = emailSubject,
                            messageBody = emailBody,
                            sent = false
                        };
                        var messageRowsAdded = connection.Execute(_insertMessageSql, messageData, transaction: trx);

                        trx.Commit();
                    }

                }
                Log.Information("SaveRequest for {id} completed successfully", id);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to complete SaveRequest for {id} due to {error}", id, ex.Message);
                throw;
            }
        }
    }
}
