using ReptileUI.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    class IniFileOperation2
    {
        /// <summary>
        /// ini存储位置
        /// </summary>
        private string _path;

        /// <summary>
        /// ini文件
        /// </summary>
        private FileInfo _file;

        /// <summary>
        /// ini配置文件内容
        /// </summary>
        private DictionaryEX<string, DictionaryEX<string, string>> _value;

        /// <summary>
        /// ini配置文件中节起始行
        /// </summary>
        private DictionaryEX<string, int> _sectionIndex;

        /// <summary>
        /// ini文件存不存在
        /// </summary>
        public bool exist
        {
            get
            {
                return _file.Exists;
            }
        }

        /// <summary>
        /// ini文件的完整路径
        /// </summary>
        public string Path
        {
            get
            {
                return _file.FullName;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="iniPath"></param>
        public IniFileOperation2(string iniPath)
        {
            _path = iniPath;
            _file = new FileInfo(_path);

            if (!_file.Exists)
            {
                throw new FileNotFoundException($"ini 配置文件未找到: {_path}");
            }

            //读取整个ini配置文件
        }

        /// <summary>
        /// 写入值到Ini文件。
        /// 写入内容会在程序关闭时写入ini文件
        /// </summary>
        /// <param name="section">要写入的节。</param>
        /// <param name="key">要写入的键。</param>
        /// <param name="value">要写入的值。</param>
        public void Write(string section,string key,string value)
        {
            
        }

        /// <summary>
        /// 立即写入值到Ini文件。
        /// </summary>
        /// <param name="section">要写入的节。</param>
        /// <param name="key">要写入的键。</param>
        /// <param name="value">要写入的值。</param>
        public void WriteNow(string section,string key,string value)
        {

        }
    }
}
