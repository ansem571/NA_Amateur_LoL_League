using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Contracts
{
    public class BaseInfo
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
