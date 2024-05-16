using ReptileUI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReptileUI
{
    static class Program
    {
        /// <summary>
        /// ini文件
        /// </summary>
        public static readonly string settingIni = "Setting.ini";

        /// <summary>
        /// ini中存放读取章节名称的正则块
        /// </summary>
        public static readonly string readDD = "RULE";

        /// <summary>
        /// ini中存放读取整段内容的正则块
        /// </summary>
        public static readonly string readText = "TEXTRULE";

        /// <summary>
        /// ini中存放读取多行内容的正则块
        /// </summary>
        public static readonly string readLine = "SECTIONRULE";

        /// <summary>
        /// ini中用于存储UI内容的块
        /// </summary>
        public static readonly string uiSetting = "UISETTING";

        /// <summary>
        /// 使用的Python工具名称
        /// </summary>
        public static readonly string TOOL_NAME = "main.exe";

        /// <summary>
        /// CMD命令结尾字符串(不使用)
        /// </summary>
        public static readonly string ENT_TEXT = "###END###";

        /// <summary>
        /// ini配置文件操作模块
        /// </summary>
        public static IniFileOperation2 iniFile;

        /// <summary>
        /// Python的Urllib3爬虫工具
        /// </summary>
        public static Python pythonGet;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
