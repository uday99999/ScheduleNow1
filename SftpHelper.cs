using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System.IO;

namespace ScheduleNow1
{
    public static class SftpHelper
    {
        public static async Task UploadFile(string localFilePath, string sftpHost, string sftpUsername, string sftpPassword, string remoteDirectory, ILogger logger)
        {
            using (var sftp = new SftpClient(sftpHost, sftpUsername, sftpPassword))
            {
                logger.LogInformation($"Uploading file {localFilePath} to SFTP server {sftpHost}");

                await Task.Run(() => sftp.Connect());
                using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                {
                   await Task.Run(() => sftp.UploadFile(fileStream, Path.Combine(remoteDirectory, Path.GetFileName(localFilePath))));
                }
                await Task.Run(() => sftp.Disconnect());
                logger.LogInformation("File upload completed.");

            }
        }
    }
}