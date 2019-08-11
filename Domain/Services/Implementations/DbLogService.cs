using System;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.Logging;
using Domain.Helpers;
using Domain.Mappers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class DbLogService : ILogger, IDisposable
    {
        private readonly ITableStorageRepository<MessageLogEntity> _tableStorageRepository;
        private readonly ILogLevelMapper _logLevelMapper;
        private readonly string _createdBy;

        public DbLogService(ITableStorageRepository<MessageLogEntity> tableStorageRepository, ILogLevelMapper logLevelMapper, string createdBy)
        {
            _tableStorageRepository = tableStorageRepository ?? throw new ArgumentNullException(nameof(tableStorageRepository));
            _logLevelMapper = logLevelMapper ?? throw new ArgumentNullException(nameof(logLevelMapper));
            _createdBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                var messageLogEntity = new MessageLogEntity
                {
                    Id = Guid.NewGuid(),
                    Timestamp = TimeZoneExtensions.GetCurrentTime(),
                    TypeId = _logLevelMapper.Map(logLevel),
                    Message = $"{formatter(state, exception)}",
                    Exception = exception?.ToString(),
                    InnerException = exception?.InnerException?.ToString(),
                    StackTrace = exception?.StackTrace,
                    Source = exception?.Source,
                    CreatedOn = TimeZoneExtensions.GetCurrentTime(),
                    CreatedBy = _createdBy
                };

                Task.Run(() =>
                {
                    _tableStorageRepository.InsertAsync(messageLogEntity);

                }).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // do nothing
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {

        }
    }
}
