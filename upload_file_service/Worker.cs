using System.IO;

namespace upload_file_service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private List<string>? _fileExtendsion;

        public Worker(ILogger<Worker> logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            if(_fileExtendsion != null) 
                _fileExtendsion.Clear();
            _fileExtendsion = null;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(2000);


            string? filepath = _configuration.GetValue<string>("FilePath");
            int intervalSendFile = _configuration.GetValue<int>("IntervalSend");
            string? fileTypeForUpload = _configuration.GetValue<string>("SendFileType");
            string? serverPath = _configuration.GetValue<string>("ServerPath");
            if (filepath == null || filepath == "" || filepath == string.Empty)
            {
                _logger.LogError("Notfound FilePath in appconfig");
                return;
            }
            if(serverPath == null || serverPath == "" || serverPath  == string.Empty)
            {
                _logger.LogError("Notfound ServerPath in appconfig");
                return;
            }
            if(fileTypeForUpload == null || fileTypeForUpload == "" || fileTypeForUpload == string.Empty)
            {
                _logger.LogError("Not provide file extendsion");
            }
            else
            {
                _fileExtendsion = fileTypeForUpload.Split('|').ToList();

                if(_fileExtendsion == null || _fileExtendsion.Count == 0)
                {
                    _logger.LogError("Not provide file extendsion");
                }
            }

            if (intervalSendFile == 0)
            {
                intervalSendFile = 1000;
            }

            _logger.LogInformation($"Directory watch : {filepath}");
            _logger.LogInformation($"Interval : {intervalSendFile}");
            _logger.LogInformation($"File Upload : {fileTypeForUpload}");
            _logger.LogInformation($"Uplaod path : {serverPath}");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var data = ListFileDirectories(filepath);
                    if(data != null && data.Count >0)
                    {
                        await FileUploader.UploadFilesAsync(serverPath, data.ToArray());


                        foreach (var file in data)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch(Exception ex)  
                            {
                                _logger.LogError($"Failed to delete file {file}");
                            }

                        }

                    }
                    data.Clear();

                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex.Message);
                }
                await Task.Delay(intervalSendFile, stoppingToken);
            }
        }

        public List<string> ListFileDirectories(string rootPath)
        {
            var fileDirectories = new List<string>();

            // Ensure the root directory exists
            if (!Directory.Exists(rootPath))
            {
                Console.WriteLine("The specified directory does not exist.");
                return fileDirectories;
            }
            var files = Directory.GetFiles(rootPath, "*", SearchOption.TopDirectoryOnly).ToList();
            var res = files.FindAll(f => _fileExtendsion.Contains(Path.GetExtension(f).ToLower()));
            return res;
        }
    }
}
