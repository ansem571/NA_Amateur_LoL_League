using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using DAL.Entities.Logging;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly ILogger _logger;
        private readonly IGoogleDriveFolderRepository _googleDriveFolderRepository;
        private readonly IEmailService _emailService;

        private readonly string[] _driveScopes =
            { DriveService.Scope.Drive, DriveService.Scope.DriveMetadata };

        private const string MyApplicationName = "Casual Esports Amateur League";
        private UserCredential _credential;
        private DriveService _driveService;
        private readonly string _wwwRootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        private readonly List<string> _folderNames = new List<string>
        {
            "Week1", "Week2", "Week3", "Week4", "Week5", "Semi-Finals", "Finals"
        };

        public GoogleDriveService(ILogger logger, IGoogleDriveFolderRepository googleDriveFolderRepository, IEmailService emailService)
        {
            _logger = logger ??
                        throw new ArgumentNullException(nameof(logger));
            _googleDriveFolderRepository = googleDriveFolderRepository ??
                        throw new ArgumentNullException(nameof(googleDriveFolderRepository));

            _emailService = emailService ??
                            throw new ArgumentNullException(nameof(emailService));
        }

        public void SetupCredentials()
        {
            if (_credential != null)
            {
                return;
            }
            var directory = Path.Combine(_wwwRootDirectory, "Auth");
            var credentialsPath = Path.Combine(directory, "credentials.json");
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                var tokenPath = Path.Combine(directory, "token.json");
                _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _driveScopes,
                    "casualesportsamateurleague@gmail.com",
                    CancellationToken.None,
                    new FileDataStore(tokenPath, true)).Result;
                _logger.LogInformation($"Credential file saved to: {tokenPath}");
            }

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
                ApplicationName = MyApplicationName
            });
        }

        public async Task<bool> CreateFolders()
        {
            var folders = await _googleDriveFolderRepository.GetAllFoldersAsync();
            var tempFolderNames = new List<string>(_folderNames);
            foreach (var folder in folders)
            {
                if (_folderNames.Contains(folder.FolderName))
                {
                    tempFolderNames.Remove(folder.FolderName);
                }
            }

            //Any remaining folders
            var newFolders = new List<GoogleDriveFolderEntity>();
            foreach (var week in tempFolderNames)
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = week,
                    MimeType = "application/vnd.google-apps.folder"
                };

                var folderRequest = _driveService.Files.Create(fileMetadata);
                folderRequest.Fields = "id";
                var folder = folderRequest.Execute();

                newFolders.Add(new GoogleDriveFolderEntity
                {
                    Id = Guid.NewGuid(),
                    FolderName = week,
                    FolderId = folder.Id
                });
            }

            return await _googleDriveFolderRepository.InsertAsync(newFolders);
        }

        public async Task<bool> SendFileData(MatchSubmissionView view)
        {
            CreateCsvDataFile(view);

            var csvFile = Path.Combine(_wwwRootDirectory, $"MatchCsvs\\{view.FileName}.csv");

            await _emailService.SendEmailAsync("casualesportsamateurleague@gmail.com", "Match result for subject", view.FileName, new List<Attachment>
            {
                new Attachment(csvFile)
            });

            return true;
        }

        private void CreateCsvDataFile(MatchSubmissionView view)
        {
            var matchCsvsDir = Path.Combine(_wwwRootDirectory, "MatchCsvs");
            if (!Directory.Exists(matchCsvsDir))
            {
                Directory.CreateDirectory(matchCsvsDir);
            }

            var csvFile = Path.Combine(_wwwRootDirectory, $"MatchCsvs\\{view.FileName}.csv");
            try
            {
                if (File.Exists(csvFile))
                {
                    File.Delete(csvFile);
                }
            }
            catch (Exception e)
            {
                var newGuid = Guid.NewGuid();
                File.Copy(csvFile, csvFile.Replace(".csv", $"{newGuid}.csv"));
                File.Delete(csvFile);
            }

            using (var writer = new StreamWriter(csvFile, false, Encoding.UTF8))
            {
                using (var csvWriter = new CsvWriter(writer))
                {
                    //csvWriter.WriteField("");

                    var gameNum = 0;
                    foreach (var gameInfo in view.GameInfos)
                    {
                        gameNum++;
                        WriteHeader(csvWriter, gameNum);
                        csvWriter.NextRecord();
                        for (var i = 0; i < 5; i++) //players
                        {
                            if (i == 0)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField(gameInfo.TeamWithSideSelection);
                                csvWriter.WriteField(gameInfo.BlueSideWinner ? "Blue" : "Red");
                                csvWriter.WriteField(gameInfo.ProdraftSpectateLink);
                                csvWriter.WriteField(gameInfo.MatchHistoryLink);

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerTop);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionTop);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerTop);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionTop);
                            }
                            else if (i == 1)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerJungle);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionJungle);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerJungle);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionJungle);
                            }

                            else if (i == 2)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerMid);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionMid);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerMid);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionMid);
                            }

                            else if (i == 3)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerAdc);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionAdc);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerAdc);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionAdc);
                            }

                            else if (i == 4)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerSup);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionSup);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerSup);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionSup);
                            }
                            csvWriter.NextRecord();
                        }
                        csvWriter.NextRecord();
                    }
                }
                writer.Close();
            }
        }

        private static void WriteHeader(IWriterRow csvWriter, int gameNum)
        {

            csvWriter.WriteField($"Game {gameNum}");
            var selected = gameNum % 2 == 0 ? "Home" : "Away";
            csvWriter.WriteField($"{selected} Side Selection");
            csvWriter.WriteField("Winner");
            csvWriter.WriteField("ProDraft Spectate Link");
            csvWriter.WriteField("Match History Link");
            csvWriter.WriteField("Blue Player");
            csvWriter.WriteField("Blue Champion");
            csvWriter.WriteField("Red Player");
            csvWriter.WriteField("Red Champion");

        }
    }
}
