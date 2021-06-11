using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jisons
{
    [Serializable]
    public class FileData
    {
        public bool IsReceive { get; set; }

        public bool IsSend { get; set; }

        public string Name { get; set; }

        public string Root { get; set; }

        public int Length { get; set; }

        public string FullName { get; set; }

        public string ParentName { get; set; }

        public string MD5 { get; set; }

        public FileData()
        {
            IsSend = IsReceive = false;
        }
    }

    [Serializable]
    public class FloderData
    {

        public List<FileData> FileDataList { get; set; }

        public string FullPath { get; set; }

        public int Length { get; set; }

        public FloderData()
        {
            FileDataList = new List<FileData>();
        }
    }

    public static class DirectoryHelper
    {

        public static List<FileData> GetAllFiles(string path)
        {
            var DirectoryInfo = new System.IO.DirectoryInfo(path);
            return DirectoryHelper.GetAllFiles("", DirectoryInfo.Name, DirectoryInfo.Parent.FullName).ToList();
        }

        public static IEnumerable<FileData> GetAllFiles(string path, string parentname = "", string rootname = "")
        {
            var realpath = Path.Combine(rootname, parentname, path);
            DirectoryInfo di = new DirectoryInfo(realpath);
            if (di.Exists)
            {
                foreach (var item in di.GetFiles())
                {
                    var p = Path.Combine(path, parentname);
                    var f = GetFileDataInfo(item.FullName, path, parentname, rootname);
                    if (f != null)
                    {
                        yield return f;
                    }
                }
                foreach (var item in di.GetDirectories())
                {
                    var pn = string.IsNullOrWhiteSpace(path) ? item.Name : path + "\\" + item.Name;
                    var fl = GetAllFiles(pn, parentname, rootname).ToList();
                    foreach (var filedatainfo in fl)
                    {
                        yield return filedatainfo;
                    }
                }
            }
        }

        public static FileData GetFileDataInfo(string filepath, string path, string parentname, string root)
        {
            FileInfo fileinfo = new FileInfo(filepath);
            if (fileinfo.Exists)
            {
                var fd = new FileData();
                fd.MD5 = MD5Helper.GetFastMD5(fileinfo.FullName);
                fd.Length = (int)fileinfo.Length;
                fd.FullName = fileinfo.FullName;
                fd.ParentName = parentname;
                fd.Root = root;
                fd.Name = string.IsNullOrWhiteSpace(path) ? fileinfo.Name : path + "\\" + fileinfo.Name;
                return fd;
            }
            return null;
        }
    }

}
