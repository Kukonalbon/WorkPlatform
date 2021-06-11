using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonHelper
{
    public static class DialogHelper
    {

        public static List<string> SearchFiles()
        {
            List<string> filPath = new List<string>();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.DereferenceLinks = true;

            dialog.Filter = "所有文件(*.*)|*.*";
            dialog.FilterIndex = 0;
            dialog.RestoreDirectory = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filPath.AddRange(dialog.FileNames);
            }
            return filPath;
        }

        public static string SearchFolder()
        {
            string filPath = string.Empty;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filPath = dialog.SelectedPath;
            }
            return filPath;
        }

        public static string SaveFile(string defaultExt = "")
        {
            string filPath = string.Empty;
            SaveFileDialog dialog = new SaveFileDialog();
            if (!string.IsNullOrWhiteSpace(defaultExt))
            {
                dialog.DefaultExt = defaultExt;
                dialog.AddExtension = true;
            }
            dialog.Filter = "所有文件(*.*)|*.*";
            dialog.FilterIndex = 0;
            dialog.DereferenceLinks = true;
            dialog.RestoreDirectory = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filPath = dialog.FileName;
            }
            return filPath;
        }


    }
}
