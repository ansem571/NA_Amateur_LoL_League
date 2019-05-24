using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.Logging
{
    [Table("GoogleDriveFolder")]
    public class GoogleDriveFolderEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string FolderId { get; set; }
        public string FolderName { get; set; }
    }
}
