using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folder2Zip.Models
{
    delegate void ZipLoadedEventHandler(object sender, int fileCount);
    delegate void ZipProgressChangedEventHandler(object sender, ZipLogItem logItem);
    delegate void ZipCompletedEventHandler(object sender, ZipCompletedStatus result);

    enum ZipCompletedStatus
    {
        Completed,
        Untreated,
        Cancel,
    }

    class ZipManager
    {
        private string _dirPath;
        private bool _compression;
        private bool _isCancel = false;
        private Task _extractTask;

        public event ZipLoadedEventHandler Loaded;
        public event ZipProgressChangedEventHandler ProgressChanged;
        public event ZipCompletedEventHandler Completed;

        public ZipManager(string dirPath, bool compression)
        {
            _dirPath = dirPath;
            _compression = compression;
        }

        public void ExtractAsync()
        {
            if (_extractTask != null)
            {
                if (!_extractTask.IsCompleted)
                {
                    return;
                }
            }

            _isCancel = false;
            _extractTask = Task.Run(() =>
            {
                if (!Directory.Exists(_dirPath))
                {
                    Completed?.Invoke(this, ZipCompletedStatus.Untreated);
                    return;
                }
                string[] dirs = Directory.GetDirectories(_dirPath);
                if (dirs.Length == 0)
                {
                    Completed?.Invoke(this, ZipCompletedStatus.Untreated);
                    return;
                }

                Loaded?.Invoke(this, dirs.Length);
                foreach (string dir in dirs)
                {
                    if (_isCancel)
                    {
                        Completed?.Invoke(this, ZipCompletedStatus.Cancel);
                        return;
                    }
                    ZipLogItem logItem = zipFolder(dir);
                    ProgressChanged?.Invoke(this, logItem);
                }

                Completed?.Invoke(this, ZipCompletedStatus.Completed);
            });
        }

        public void Cancel()
        {
            _isCancel = true;
        }

        private ZipLogItem zipFolder(string dirPath)
        {
            string outputPath = dirPath + ".zip";
            if (File.Exists(outputPath))
            {
                return new ZipLogItem()
                {
                    FilePath = dirPath,
                    Message = "展開先に既にファイルが存在しています",
                    IsSuccess = false,
                };
            }

            try
            {
                CompressionLevel level = _compression ? CompressionLevel.Fastest : CompressionLevel.NoCompression;
                ZipFile.CreateFromDirectory(dirPath, outputPath, level, false);
            }
            catch (Exception e)
            {
                return new ZipLogItem()
                {
                    FilePath = dirPath,
                    Message = "例外発生" + e.Message,
                    IsSuccess = false,
                };
            }

            return new ZipLogItem()
            {
                FilePath = dirPath,
                Message = "成功",
                IsSuccess = true,
            };
        }
    }
}
