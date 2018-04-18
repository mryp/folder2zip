using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Folder2Zip.ViewModels
{
    public class StartViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private string _folderPath;
        private bool _compression;

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                NotifyOfPropertyChange(() => FolderPath);
            }
        }

        public bool Compression
        {
            get { return _compression; }
            set
            {
                _compression = value;
                NotifyOfPropertyChange(() => Compression);
            }
        }

        public StartViewModel(INavigationService navigationService)
        {
            this._navigationService = navigationService;

            this.FolderPath = "";
        }

        public void SetSelectFolder()
        {
            //https://github.com/aybe/Windows-API-Code-Pack-1.1 を使用
            var dialog = new CommonOpenFileDialog("フォルダ選択")
            {
                IsFolderPicker = true,
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.FolderPath = dialog.FileName;
            }
        }


        public void Start()
        {
            if (!Directory.Exists(this.FolderPath))
            {
                showError("指定したフォルダが見つかりません");
                return;
            }

            _navigationService.For<ProcViewModel>()
                .WithParam(v => v.FolderPath, FolderPath)
                .WithParam(v => v.Compression, Compression)
                .Navigate();
        }

        private void showError(string message)
        {
            MessageBox.Show(message, "エラー"
                , MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}
