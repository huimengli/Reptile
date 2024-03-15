using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReptileUI.Tools
{
    /// <summary>
    /// ini文件操作模块
    /// </summary>
    /// <example>
    /// // 使用示例
    /// var iniFile = new IniFile("path_to_your_ini_file.ini");
    /// iniFile.Write("SectionName", "KeyName", "Value");
    /// string value = iniFile.Read("SectionName", "KeyName");
    /// Console.WriteLine(value);
    /// </example>
    class IniFileOperation
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder value, int size, string filePath);

        private string _path;

        private FileInfo file;

        public bool exist
        {
            get
            {
                return file.Exists;
            }
        }

        public string Path{
            get
            {
                return file.FullName;
            }
        }

        public IniFileOperation(string iniPath)
        {
            _path = iniPath;
            file = new FileInfo(_path);
        }

        /// <summary>
        /// 写入值到Ini文件。
        /// </summary>
        /// <param name="section">要写入的节。</param>
        /// <param name="key">要写入的键。</param>
        /// <param name="value">要写入的值。</param>
        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _path);
        }

        /// <summary>
        /// 从Ini文件读取值。
        /// </summary>
        /// <param name="section">要读取的节。</param>
        /// <param name="key">要读取的键。</param>
        /// <returns>读取到的值。</returns>
        public string Read(string section, string key)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", SB, 255, _path);
            return SB.ToString();
        }

        /// <summary>
        /// 从Ini文件读取值。
        /// </summary>
        /// <param name="section">要读取的节。</param>
        /// <param name="key">要读取的键。</param>
        /// <param name="defaultValue">找不到键时返回的默认值。</param>
        /// <returns>读取到的值或默认值。</returns>
        public string Read(string section, string key, string defaultValue)
        {
            StringBuilder SB = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, SB, 255, _path);
            return SB.ToString();
        }

        /// <summary>
        /// 新增方法来读取整个节
        /// </summary>
        public Dictionary<string, string> ReadSection(string section)
        {
            const int bufferSize = 2048;
            StringBuilder buffer = new StringBuilder(bufferSize);
            GetPrivateProfileString(section, null, null, buffer, bufferSize, _path);

            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] tmp = buffer.ToString().Split('\0');
            foreach (string entry in tmp)
            {
                if (entry.Length > 0)
                {
                    int index = entry.IndexOf('=');
                    if (index > 0 && index < entry.Length)
                    {
                        result[entry.Substring(0, index)] = entry.Substring(index + 1);
                    }
                }
            }
            return result;
        }
    }
}
