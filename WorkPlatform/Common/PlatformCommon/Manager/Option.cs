using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlatformCommon.Manager
{
    public static class Option
    {
        static Option()
        {
            Init();
        }

        /// <summary> 用户配置文件名 </summary>
        public const string SoftwareName = "WorkPlatform";

        /// <summary> 用户配置文件路径 </summary>
        public static string UserCustomerConfigFloder { get; private set; }

        /// <summary> 当前版本 </summary>
        public static Version EditorVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        private static void Init()
        {
            UserCustomerConfigFloder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SoftwareName);
            CheckUserConfigDir();
        }

        private static void CheckUserConfigDir()
        {
            CheckDirToCreat(UserCustomerConfigFloder);
        }

        public static void CheckDirToCreat(string dirPath)
        {
            if (Directory.Exists(dirPath) == false)
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (System.Exception ex)
                {
                    GlobalLog.Output.Error("创建配置目录失败", ex);
                }
            }
        }
    }
}
