using System;
using Domain.Mappers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Mappers.Implementations
{
    public class LogLevelMapper: ILogLevelMapper
    {
        private readonly Guid _information = new Guid("FE036D49-AFA9-4644-AAE3-19BC1F051647");
        private readonly Guid _warning = new Guid("A574179B-9544-4938-9ED3-209E7B34564A");
        private readonly Guid _error = new Guid("713AD538-E966-4403-9AF2-943C29647610");
        public Guid Map(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Information:
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return _information;
                case LogLevel.Warning:
                    return _warning;
                case LogLevel.Error:
                case LogLevel.Critical:
                    return _error;
                case LogLevel.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }

            throw new ArgumentOutOfRangeException(nameof(logLevel));
        }
    }
}
