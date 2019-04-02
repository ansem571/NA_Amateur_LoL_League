using System;
using Microsoft.Extensions.Logging;

namespace Domain.Mappers.Interfaces
{
    public interface ILogLevelMapper
    {
        Guid Map(LogLevel logLevel);
    }
}
