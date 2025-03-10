﻿using ReptileUI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReptileUI.Class;
using ReptileUI.Enums;
using System.Windows.Forms;

namespace ReptileUI.Rule
{
    /// <summary>
    /// 读取文件的正则表达式
    /// </summary>
    public class ReadRules
    {
        #region 固定抬头值

        /// <summary>
        /// 读取章节的头部
        /// </summary>
        private static readonly string READ_DD = "readDD";

        /// <summary>
        /// 读取大段的头部
        /// </summary>
        private static readonly string READ_TEXT = "text";

        /// <summary>
        /// 读取多行的头部
        /// </summary>
        private static readonly string READ_LINE = "text";

        #endregion

        #region 存储数据位置
        /// <summary>
        /// 读取章节的正则存储
        /// </summary>
        private List<Regex> readDDs = new List<Regex>();

        /// <summary>
        /// 读取大段文章的正则存储
        /// </summary>
        private List<Regex> readTexts = new List<Regex>();

        /// <summary>
        /// 读取多行文章的正则存储
        /// </summary>
        private List<Regex> readLines = new List<Regex>();

        #endregion

        /// <summary>
        /// 给外部访问用的
        /// </summary>
        public List<Regex> ReadDDs
        {
            get
            {
                return new List<Regex>(readDDs);
            }
        }

        /// <summary>
        /// 给外部访问用的
        /// </summary>
        public List<Regex> ReadTexts
        {
            get
            {
                return new List<Regex>(readTexts);
            }
        }

        /// <summary>
        /// 给外部访问用的
        /// </summary>
        public List<Regex> ReadLines
        {
            get
            {
                return new List<Regex>(readLines);
            }
        }

        /// <summary>
        /// 获取读取章节的正则
        /// </summary>
        /// <param name="index"></param>
        public Regex GetReadDD(int index)
        {
            try
            {
                return readDDs[index];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取读取大段文章的正则表达式
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Regex GetReadText(int index)
        {
            try
            {
                return readTexts[index];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取读取多行文章的正则表达式
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Regex GetReadLine(int index)
        {
            try
            {
                return readLines[index];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public ReadRules()
        {
            var text = Program.iniFile.Read(Program.uiSetting, "file");

            var read = new Regex(READ_DD+@"[\d]+");
            var dict = Program.iniFile.ReadSection(Program.readDD);
            readDDs = dict
                .Filter((k,v)=> read.IsMatch(k))
                .ValueList()
                .Amplify(t=>new Regex(t));

            read = new Regex(READ_TEXT+@"[\d]+");
            dict = Program.iniFile.ReadSection(Program.readText);
            readTexts = dict
                .Filter((k, v) => read.IsMatch(k))
                .ValueList()
                .Amplify(t => new Regex(t));

            read = new Regex(READ_LINE + @"[\d]+");
            dict = Program.iniFile.ReadSection(Program.readLine);
            readLines = dict
                .Filter((k, v) => read.IsMatch(k))
                .ValueList()
                .Amplify(t => new Regex(t));
        }

        /// <summary>
        /// 添加读取章节的正则
        /// </summary>
        /// <param name="readDD"></param>
        public void AddDD(string regex)
        {
            readDDs.Add(Item.CreateRegex(regex));
        }

        /// <summary>
        /// 添加读取大段文章的正则
        /// </summary>
        /// <param name="regex"></param>
        public void AddText(string regex)
        {
            readTexts.Add(Item.CreateRegex(regex));
        }

        /// <summary>
        /// 添加读取多行的正则
        /// </summary>
        /// <param name="regex"></param>
        public void AddLine(string regex)
        {
            readLines.Add(Item.CreateRegex(regex));
        }

        /// <summary>
        /// 将正则保存进INI文件
        /// </summary>
        public void Save()
        {
            // 清空相关节内容
            Program.iniFile.Clear(Program.readDD);
            Program.iniFile.Clear(Program.readText);
            Program.iniFile.Clear(Program.readLine);

            // 写入读取章节的正则内容
            readDDs.ToDictionaryEX((regex, index) =>
            {
                return $"{READ_DD}{index}";
            }).ForEach(item =>
            {
                Program.iniFile.Write(Program.readDD, item.Key, Item.RegexToIni(item.Value));
            });

            // 写入读取整段的正则内容
            readTexts.ToDictionaryEX((regex, index) =>
            {
                return $"{READ_TEXT}{index}";
            }).ForEach(item =>
            {
                Program.iniFile.Write(Program.readText, item.Key, Item.RegexToIni(item.Value));
            });

            // 写入读取多行的正则内容
            readLines.ToDictionaryEX((regex, index) => $"{READ_LINE}{index}")
            .ForEach(item =>
            {
                Program.iniFile.Write(Program.readLine, item.Key, Item.RegexToIni(item.Value));
            });

            // 保存ini数据
            Program.iniFile.Save();
        }

        /// <summary>
        /// 选择某个模块刷新并保存
        /// </summary>
        /// <param name="enum"></param>
        public void Save(ReadRuleEnum @enum)
        {
            switch (@enum)
            {
                case ReadRuleEnum.READ_DD:
                    Program.iniFile.Clear(Program.readDD);

                    // 写入读取章节的正则内容
                    readDDs.ToDictionaryEX((regex, index) =>
                    {
                        return $"{READ_DD}{index}";
                    }).ForEach(item =>
                    {
                        Program.iniFile.Write(Program.readDD, item.Key, Item.RegexToIni(item.Value));
                    });
                    break;
                case ReadRuleEnum.READ_TEXT:
                    Program.iniFile.Clear(Program.readText);

                    // 写入读取整段的正则内容
                    readTexts.ToDictionaryEX((regex, index) =>
                    {
                        return $"{READ_TEXT}{index}";
                    }).ForEach(item =>
                    {
                        Program.iniFile.Write(Program.readText, item.Key, Item.RegexToIni(item.Value));
                    });
                    break;
                case ReadRuleEnum.READ_LINE:
                    Program.iniFile.Clear(Program.readLine);

                    // 写入读取多行的正则内容
                    readLines.ToDictionaryEX((regex, index) => $"{READ_LINE}{index}")
                    .ForEach(item =>
                    {
                        Program.iniFile.Write(Program.readLine, item.Key, Item.RegexToIni(item.Value));
                    });
                    break;
                case ReadRuleEnum.NONE:
                default:
                    MessageBox.Show("错误的模块选择!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            // 保存ini数据
            Program.iniFile.Save();
        }
    }
}
