using ReptileUI.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ReptileUI.Tools
{
    /// <summary>
    /// ini文件操作模块
    /// </summary>
    /// <example>
    /// // 使用示例
    /// var iniFile = new IniFileOperation2("path_to_your_ini_file.ini");
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
        /// 内部使用:读取用
        /// </summary>
        private StreamReader _sr;

        /// <summary>
        /// 内部使用:写入用
        /// </summary>
        private StreamWriter _sw;

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
            using (_sr = new StreamReader(_file.OpenRead(),new UTF8Encoding(false)))
            {
                ReadAll();
            }

            //_sw = new StreamWriter(_file.OpenWrite(), Encoding.UTF8);
        }

        /// <summary>
        /// 将INI数据保存到本地
        /// </summary>
        public void Save()
        {
            var file = _file.Create();
            file.Close();
            using (_sw = new StreamWriter(_file.OpenWrite(), new UTF8Encoding(false)))
            {
                WriteAll();
            }
        }

        /// <summary>
        /// 关闭并保存INI配置文件
        /// </summary>
        public void Close()
        {
            Save();
            _file = null;
        }

        #region 内部使用函数

        /// <summary>
        /// 内部使用:读取整个文件
        /// </summary>
        private void ReadAll()
        {
            string line;
            string section = "";
            Regex readSection = new Regex(@"^\[([^\]]+)\]$");
            // 更新正则表达式，允许在值后面跟随注释
            Regex readKeyValue = new Regex(@"^([^=]+)=([^\;#]*)");
            _value = new DictionaryEX<string, DictionaryEX<string, string>>();
            DictionaryEX<string, string> _section = null;
            _sectionIndex = new DictionaryEX<string, int>();
            int index = 0;

            while ((line = _sr.ReadLine()) != null)
            {
                line = line.Trim();
                index++;
                if (line.StartsWith(";") || line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                {
                    continue; // 跳过注释和空行
                }

                Match match = readSection.Match(line);
                if (match.Success)
                {
                    section = match.Groups[1].Value;
                    if (!_value.ContainsKey(section))
                    {
                        _section = new DictionaryEX<string, string>();
                        _value.Add(section, _section);
                        _sectionIndex.Add(section, index);
                    }
                }
                else
                {
                    match = readKeyValue.Match(line);
                    if (match.Success)
                    {
                        string key = match.Groups[1].Value.Trim();
                        string value = match.Groups[2].Value.Trim();

                        // 移除值后面的注释（如果有）
                        int commentIndex = value.IndexOf(';');
                        if (commentIndex == -1)
                        {
                            commentIndex = value.IndexOf('#');
                        }
                        if (commentIndex != -1)
                        {
                            value = value.Substring(0, commentIndex).Trim();
                        }

                        if (_section==null)
                        {
                            if (_value[""]==null)
                            {
                                _section = new DictionaryEX<string, string>();
                                _value.Add("", _section);
                            }
                            else
                            {
                                _section = _value[""];
                            }
                        }

                        if (!_section.ContainsKey(key))
                        {
                            _section.Add(key, value);
                        }
                        else
                        {
                            _section[key] = value; // 更新已存在的键
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 内部使用:将所有数据写入文件
        /// </summary>
        private void WriteAll()
        {
            // 写入无节头数据
            if (_value[""]!=null)
            {
                //写入节内容
                foreach (var item in _value[""])
                {
                    _sw.WriteLine($"{item.Key}={item.Value}");
                }
                //写入空行
                _sw.WriteLine();
            }
            // 写入有节头数据
            foreach (var section in _value)
            {
                if (!String.IsNullOrWhiteSpace(section.Key))
                {
                    //写入节头
                    _sw.WriteLine($"[{section.Key.ToUpper()}]");
                    //写入节内容
                    foreach (var item in section.Value)
                    {
                        _sw.WriteLine($"{item.Key}={item.Value}");
                    }
                    //写入空行
                    _sw.WriteLine();
                }
            }
        }
        #endregion

        /// <summary>
        /// 创建Ini节
        /// </summary>
        /// <param name="sectionName"></param>
        public void CreateSection(string sectionName)
        {
            sectionName = sectionName.Trim().ToUpper();
            if (_file==null)
            {
                Item.Log("配置文件已关闭!");
                return;
            }
            if (_value[sectionName]!=null)
            {
                Item.Log("ini节已经存在!");
                return;
            }
            else
            {
                _value.Add(sectionName, new DictionaryEX<string, string>());
            }
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
            section = section.Trim().ToUpper();
            if (_file == null)
            {
                Item.Log("配置文件已关闭!");
                return;
            }
            var _section = _value[section];
            if (_section==null)
            {
                _section = new DictionaryEX<string, string>();
                _value.Add(section, _section);
            }
            var oldVal = _section[key];
            if (oldVal==null)
            {
                _section.Add(key, value);
            }
        }

        /// <summary>
        /// 立即写入值到Ini文件。
        /// </summary>
        /// <param name="section">要写入的节。</param>
        /// <param name="key">要写入的键。</param>
        /// <param name="value">要写入的值。</param>
        public void WriteNow(string section,string key,string value)
        {
            section = section.Trim().ToUpper();
            if (_file == null)
            {
                Item.Log("配置文件已关闭!");
                return;
            }
            var _section = _value[section];
            if (_section == null)
            {
                _section = new DictionaryEX<string, string>();
                _value.Add(section, _section);
            }
            var oldVal = _section[key];
            if (oldVal == null)
            {
                _section.Add(key, value);
            }
            Save();
        }

        /// <summary>
        /// 从ini文件读取值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Read(string section,string key)
        {
            section = section.Trim().ToUpper();
            if (_file == null)
            {
                Item.Log("配置文件已关闭!");
                return "";
            }
            var _section = _value[section];
            if (_section==null)
            {
                return null;
            }
            else
            {
                return _section[key];
            }
        }

        /// <summary>
        /// 从ini文件中读取值
        /// 如果ini中没有内容则返回<code>defaultValue</code>
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string Read(string section,string key,string defaultValue)
        {
            section = section.Trim().ToUpper();
            if (_file == null)
            {
                Item.Log("配置文件已关闭!");
                return "";
            }
            var _section = _value[section];
            if (_section==null)
            {
                return defaultValue;
            }
            else
            {
                return _section[key] ?? defaultValue;
            }
        }

        /// <summary>
        /// 读取ini整个节
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public DictionaryEX<string,string> ReadSection(string section)
        {
            section = section.Trim().ToUpper();
            if (_file == null)
            {
                Item.Log("配置文件已关闭!");
                return null;
            }
            return _value[section]??new DictionaryEX<string, string>();
        }
    }
}
