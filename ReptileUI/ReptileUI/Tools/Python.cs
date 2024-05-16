using ReptileUI.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReptileUI.Tools
{
    /// <summary>
    /// 利用python的exe进行网页爬取
    /// </summary>
    class Python
    {
        /// <summary>
        /// 访问的URL
        /// </summary>
        private string Url = "";

        /// <summary>
        /// 输出文件
        /// </summary>
        private string OutputFile = "";

        /// <summary>
        /// 访问代理路径
        /// </summary>
        private string Proxy = "";

        /// <summary>
        /// 最大错误次数
        /// </summary>
        private int? MaxErrorTimes = null;

        /// <summary>
        /// 临时读取内容结束
        /// </summary>
        private string EndText = "";

        /// <summary>
        /// 是否需要代理
        /// </summary>
        private bool NeedProxy = false;

        /// <summary>
        /// 是否需要验证SSL证书
        /// </summary>
        private bool NeedVerify = false;

        /// <summary>
        /// 设置访问URl
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Python SetUrl(string url)
        {
            Url = url;
            return this;
        }

        /// <summary>
        /// 设置临时输出文件
        /// </summary>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public Python SetOutput(string outputPath)
        {
            OutputFile = outputPath;
            return this;
        }

        /// <summary>
        /// 设置输出文件
        /// </summary>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public Python SetOutput(FileInfo outputFile)
        {
            OutputFile = outputFile.FullName;
            return this;
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public Python SetProxy(string proxy)
        {
            NeedProxy = true;
            Proxy = proxy;
            return this;
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <returns></returns>
        public Python SetProxy()
        {
            NeedProxy = true;
            return this;
        }

        /// <summary>
        /// 设置最大重复尝试次数
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public Python SetMaxErrorTime(int max)
        {
            MaxErrorTimes = max;
            return this;
        }

        /// <summary>
        /// 设置临时结束字符串
        /// </summary>
        /// <param name="endText"></param>
        /// <returns></returns>
        public Python SetEndText(string endText)
        {
            EndText = endText;
            return this;
        }

        /// <summary>
        /// 设置需要SSL验证
        /// </summary>
        /// <returns></returns>
        public Python SetNeedVerify()
        {
            NeedVerify = true;
            return this;
        }

        /// <summary>
        /// 设置是否需要SSL验证
        /// </summary>
        /// <param name="needVerify"></param>
        /// <returns></returns>
        public Python SetNeedVerify(bool needVerify)
        {
            NeedVerify = needVerify;
            return this;
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(Url))
            {
                throw new PythonException("访问的URL不存在!");
            }

            var ret = new StringBuilder();

            ret.Append(Program.TOOL_NAME);
            ret.Append(" -U ");
            ret.Append(Url);

            if (String.IsNullOrWhiteSpace(OutputFile)==false)
            {
                ret.Append(" -O ");
                ret.Append(OutputFile);
            }
            if (NeedProxy)
            {
                ret.Append(" -NP");
                if (String.IsNullOrWhiteSpace(Proxy)==false)
                {
                    ret.Append(" -P ");
                    ret.Append(Proxy);
                }
            }
            if (MaxErrorTimes!=null)
            {
                ret.Append(" -M ");
                ret.Append(MaxErrorTimes);
            }
            if (string.IsNullOrWhiteSpace(EndText)==false)
            {
                ret.Append(" -E ");
                ret.Append(EndText);
            }
            if (NeedVerify)
            {
                ret.Append(" -NV");
            }

            return ret.ToString();
        }

        /// <summary>
        /// 测试爬取章节
        /// </summary>
        /// <returns></returns>
        public MatchCollection TestDD(Regex readDD)
        {
            var html = Item.UseCMD(ToString());
            Console.WriteLine(html);
            return readDD.Matches(html);
        }

        /// <summary>
        /// 爬取指定网页内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Get(string url)
        {
            this.SetUrl(url);
            Item.Log(ToString());
            var html = Item.UseCMD2(ToString());
            return html;
        }
    }
}
