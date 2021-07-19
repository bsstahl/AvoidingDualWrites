using System;
using System.Data.SqlClient;
using Dapper;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace RequestService
{
    public class RequestRepository : IRequestRepository
    {
        const string _insertRequestSql = "insert into tblRequests (Id, CustomerEmail, Description) values (@id, @customerEmail, @description)";
        const string _insertMessageSql = "insert into tblMessages (Id, RequestId, SendToAddress, MessageSubject, MessageBody, Sent) values (@id, @requestId, @sendToAddress, @messageSubject, @messageBody, @sent)";
        const string _connection = "Server=tcp:{0}.database.windows.net,1433;Initial Catalog={1};Persist Security Info=False;User ID={2};Password={3};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly string _connectionString;

        public RequestRepository(IConfiguration config)
        {
            _connectionString = string.Format(_connection, config["DBServerName"], config["DBDatabaseName"], config["DBUsername"], config["DBPassword"]);
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
