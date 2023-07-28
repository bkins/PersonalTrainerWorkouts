using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace PersonalTrainerWorkouts.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;

        public GoogleDriveService(DriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task<string> CreateFile()
        {
            var metadata = new File()
                           {
                               Parents  = new List<string>{ "root" }
                             , MimeType = "text/plain"
                             , Name     = "Untitled file"
                           };

            var googleFile = await _driveService.Files
                                                .Create(metadata)
                                                .ExecuteAsync();

            if (googleFile == null)
            {
                throw new System.IO.IOException("Null result when requesting file creation.");
            }

            return googleFile.Id;
        }

        public async Task<FileData> ReadFile(string fileId)
        {
            var data     = new FileData();

            await SetDataName(fileId
                            , data);

            await SetDataContent(fileId
                               , data);
            return data;
        }

        private async Task SetDataContent(string   fileId
                                        , FileData data)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                await _driveService.Files.Get(fileId)
                                   .DownloadAsync(memoryStream)
                                   .ConfigureAwait(false);

                memoryStream.Position = 0;

                using (var streamReader = new System.IO.StreamReader(memoryStream
                                                                   , Encoding.Default))
                {
                    var content = streamReader.ReadToEnd();
                    data.Content = content;
                }
            }
        }

        private async Task SetDataName(string   fileId
                                     , FileData data)
        {
            if (_driveService != null)
            {
                var metadata = await _driveService.Files.Get(fileId)
                                                  .ExecuteAsync();

                data.Name = metadata.Name;
            }
        }

        public async Task SaveFile(string fileId
                                 , string name
                                 , string content)
        {
            File metadata = new File{ Name = name };

            byte[] byteArray = Encoding.Default.GetBytes(content);

            await _driveService.Files
                               .Update(metadata
                                     , fileId
                                     , new System.IO.MemoryStream(byteArray)
                                     , "text/plain")
                               .UploadAsync()
                               .ConfigureAwait(false);
        }
    }


    public class FileData
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
