using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.Logging
{
    [Table("MessageLog")]
    public class MessageLogEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        private Guid _sessionId;// unused
        public Guid SessionId
        {
            get => Guid.Empty;
            set => _sessionId = value;
        }
        public DateTime Timestamp { get; set; }
        public Guid TypeId { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
