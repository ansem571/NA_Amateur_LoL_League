using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.Logging;

namespace Domain.Repositories.Interfaces
{
    public interface IGoogleDriveFolderRepository
    {
        Task<GoogleDriveFolderEntity> GetFolderByGoogleIdAsync(Guid googleFolderId);
        Task<GoogleDriveFolderEntity> GetFolderByFolderNameAsync(string folderName);
        Task<IEnumerable<GoogleDriveFolderEntity>> GetAllFoldersAsync();
        Task<bool> InsertAsync(IEnumerable<GoogleDriveFolderEntity> folders);
    }
}
