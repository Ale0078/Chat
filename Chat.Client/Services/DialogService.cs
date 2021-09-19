using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

using Chat.Client.Services.Interfaces;

namespace Chat.Client.Services
{
    internal class DialogService : IDialogService
    {
        public string OpenFile(string caption, string filter = "All files (*.*)|*.*")
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Title = caption,
                Filter = filter,
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog().Value)
            {
                return dialog.FileName;
            }

            return string.Empty;
        }
    }
}
