using OpenSleigh.Core.Persistence;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OpenSleigh.Persistence.Mongo
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoClient _client;
        private readonly ILogger<MongoUnitOfWork> _logger;

        public MongoUnitOfWork(IMongoClient client, ILogger<MongoUnitOfWork> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<ITransaction> StartTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transactionOptions = new TransactionOptions(ReadConcern.Majority, ReadPreference.Primary, WriteConcern.WMajority);

            ITransaction transaction;
            try
            {
                //TODO: the async overload might freeze if transactions are not supported
                var session = _client.StartSession();
                session.StartTransaction(transactionOptions);
                transaction = new MongoTransaction(session);
            }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning($"unable to start MongoDB transaction : {ex.Message}");
                transaction = new NullTransaction();
            }

            return Task.FromResult(transaction);
        }
    }
}