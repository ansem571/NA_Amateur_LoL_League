using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.Logging;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class GoogleDriveFolderRepository : IGoogleDriveFolderRepository
    {
        private readonly ITableStorageRepository<GoogleDriveFolderEntity> _table;
        public GoogleDriveFolderRepository(ITableStorageRepository<GoogleDriveFolderEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<GoogleDriveFolderEntity> GetFolderByGoogleIdAsync(Guid googleFolderId)
        {
            return (await _table.ReadManyAsync("FolderId = @googleFolderId", new {googleFolderId}, top: 1))
                .FirstOrDefault();
        }

        public async Task<GoogleDriveFolderEntity> GetFolderByFolderNameAsync(string folderName)
        {
            return (await _table.ReadManyAsync("FolderName = @folderName", new { folderName }, top: 1))
                .FirstOrDefault();
        }

        public async Task<bool> InsertAsync(IEnumerable<GoogleDriveFolderEntity> folders)
        {
            folders = folders.ToList();
            return !folders.Any() || await _table.InsertAsync(folders) == folders.Count();
        }

        public async Task<IEnumerable<GoogleDriveFolderEntity>> GetAllFoldersAsync()
        {
            return await _table.ReadAllAsync();
        }
    }
}
