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
        /// 初始化
        /// </summary>
        public ReadRules()
        {
            var read = new Regex(@"readDD[\d+]");
            var dict = Program.iniFile.ReadSection(Program.readDD);
            readDDs = dict
                .Filter((k,v)=> read.IsMatch(k))
                .ValueList()
                .Amplify(t=>new Regex(t));
        }
    }
}
