using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly ILogger _logger;
        static string[] DriveScopes =
            { DriveService.Scope.Drive, DriveService.Scope.DriveMetadata };
        static string MyApplicationName =
            "Casual Esports Amateur League";
        public GoogleDriveService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendFileData()
        {
            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    DriveScopes,
                    "casualesportsamateurleague@gmail.com",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                _logger.LogInformation($"Credential file saved to: {credPath}");
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = MyApplicationName
            });

            var listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

            _logger.LogInformation("Files");

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    _logger.LogInformation("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                _logger.LogInformation("No files found in your Google Drive.");
            }
            //C:\Users\MRMacDonnell\Desktop\Documents\Pics\log1.JPG

            var path = @"C:\Users\MRMacDonnell\Desktop\Documents\Pics\log1.JPG";

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = "TestFolder",
                MimeType = "application/vnd.google-apps.folder"
            };
            var folderRequest = service.Files.Create(fileMetadata);
            folderRequest.Fields = "id";
            var folder = folderRequest.Execute();
            Console.WriteLine("Folder ID: " + folder.Id);

            var fileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = "NotRandom",
                MimeType = MimeTypes.GetMimeType(path),
                Parents = new List<string>
                {
                    folder.Id
                }
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                request = service.Files.Create(
                    fileMetaData, stream, "image/jpeg");
                request.Fields = "id";
                var r = request.UploadAsync().Result;
                Console.WriteLine($"Exception: {r.Exception}");
            }

            var fileResponse = request.ResponseBody;
            Console.WriteLine("File ID: " + fileResponse.Id);


            Console.ReadLine();

            return true;
        }
    }
}
