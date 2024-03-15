using ReptileUI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReptileUI.Rule
{
    /// <summary>
    /// 读取文件的正则表达式
    /// </summary>
    public class ReadRules
    {
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
        /// 添加读取章节的正则表达式
        /// </summary>
        /// <param name="regex"></param>
        public void AddReadDD(Regex regex)
        {
            readDDs.Add(regex);
            Program.iniFile.Write(
                Program.readDD,
                $"readDD{readDDs.Count}",
                regex.ToString()
            );
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
        }

        /// <summary>
        /// 添加读取大段文章的正则表达式
        /// </summary>
        /// <param name="regex"></param>
        public void AddReadText(Regex regex)
        {
            readTexts.Add(regex);
            Program.iniFile.Write(
                Program.readText,
                $"text{readTexts.Count}",
                regex.ToString()
            );
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
        /// 添加读取多行文章的正则表达式
        /// </summary>
        /// <param name="regex"></param>
        public void AddReadLine(Regex regex)
        {
            readLines.Add(regex);
            Program.iniFile.Write(
                Program.readLine,
                $"text{readLines.Count}",
                regex.ToString()
            );
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

            var read = new Regex(@"readDD[\d]+");
            var dict = Program.iniFile.ReadSection(Program.readDD);
            readDDs = dict
                .Filter((k,v)=> read.IsMatch(k))
                .ValueList()
                .Amplify(t=>new Regex(t));

            read = new Regex(@"text[\d]+");
            dict = Program.iniFile.ReadSection(Program.readText);
            readTexts = dict
                .Filter((k, v) => read.IsMatch(k))
                .ValueList()
                .Amplify(t => new Regex(t));

            dict = Program.iniFile.ReadSection(Program.readLine);
            readLines = dict
                .Filter((k, v) => read.IsMatch(k))
                .ValueList()
                .Amplify(t => new Regex(t));
        }
    }
}
