using Caliburn.Micro;
using Folder2Zip.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Folder2Zip.ViewModels
{
    public class ProcViewModel : Screen
    {
        private const string BUTTON_NAME_CANCEL = "キャンセル";
        private const string BUTTON_NAME_BACK = "戻る";

        private readonly INavigationService _navigationService;
        private string _cancelButtonName;
        private int _progressMax;
        private int _progressValue;
        private object _selectedLogItem;
        private ZipManager _zip;

        public string FolderPath
        {
            get;
            set;
        }
        public bool Compression
        {
            get;
            set;
        }

        public string CancelButtonName
        {
            get { return _cancelButtonName; }
            set
            {
                _cancelButtonName = value;
                NotifyOfPropertyChange(() => CancelButtonName);
            }
        }

        public int ProgressMax
        {
            get { return _progressMax; }
            set
            {
                _progressMax = value;
                NotifyOfPropertyChange(() => ProgressMax);
            }
        }

        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                NotifyOfPropertyChange(() => ProgressValue);
            }
        }

        public object SelectedLogItem
        {
            get { return _selectedLogItem; }
            set
            {
                _selectedLogItem = value;
                NotifyOfPropertyChange(() => SelectedLogItem);
            }
        }

        public BindableCollection<ZipLogItem> ProgressItemList
        {
            get;
            set;
        }

        public ProcViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.CancelButtonName = BUTTON_NAME_CANCEL;
            this.ProgressItemList = new BindableCollection<ZipLogItem>();
        }

        public void Loaded()
        {
            _zip = new ZipManager(this.FolderPath, this.Compression);
            _zip.Loaded += unzip_Loaded;
            _zip.ProgressChanged += unzip_ProgressChanged;
            _zip.Completed += unzip_Completed;
            _zip.ExtractAsync();
        }

        public void Cancel()
        {
            if (this.CancelButtonName == BUTTON_NAME_CANCEL)
            {
                _zip.Cancel();
            }
            else
            {
                _navigationService.For<StartViewModel>().Navigate();
            }
        }

        public void CopyLogItem()
        {
            if (SelectedLogItem is ZipLogItem)
            {
                ZipLogItem item = (ZipLogItem)SelectedLogItem;
                Clipboard.SetData(DataFormats.Text, item.FilePath);
                MessageBox.Show(Path.GetFileName(item.FilePath) + "のパスをクリップボードにコピーしました。");
            }
        }

        private void unzip_Loaded(object sender, int fileCount)
        {
            this.ProgressValue = 0;
            this.ProgressMax = fileCount;
        }

        private void unzip_ProgressChanged(object sender, ZipLogItem logItem)
        {
            Debug.WriteLine(logItem);
            this.ProgressValue = this.ProgressValue + 1;
            ProgressItemList.Add(logItem);
        }

        private void unzip_Completed(object sender, ZipCompletedStatus result)
        {
            switch (result)
            {
                case ZipCompletedStatus.Completed:
                    MessageBox.Show("すべてのフォルダの処理が完了しました");
                    break;
                case ZipCompletedStatus.Untreated:
                    MessageBox.Show("処理を行うフォルダがありません");
                    break;
                case ZipCompletedStatus.Cancel:
                    MessageBox.Show("キャンセルされました");
                    break;
            }
            this.CancelButtonName = BUTTON_NAME_BACK;
        }
    }
}
