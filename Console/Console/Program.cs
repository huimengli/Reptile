using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        /// <summary>
        /// 读取横线的正则
        /// </summary>
        public static Regex read_ = new Regex(@"/-");

        static void Main(string[] args)
        {
            var color = ConsoleColor.White;
            var value = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Replace('/','-')=="-?"||args[i].Replace('/','-').ToUpper()=="-HELP")
                {
                    System.Console.WriteLine(@"
[].exe [Value] [Value2] ... -C [Color]  有色打印内容
[].exe -?                               获取帮助
                    ");
                }
                else if(args[i].Replace('/','-')=="-C"||args[i].Replace('/','-').ToUpper()=="-COLOR")
                {
                    try
                    {
                        color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), args[i + 1]);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), args[i + 1].ToUpperFirst());
                        }
                        catch (Exception)
                        {
                            WriteColor("[Error] 错误的颜色样式\n可用的颜色样式有:\n", ConsoleColor.Red);
                            foreach (ConsoleColor item in Enum.GetValues(typeof(ConsoleColor)))
                            {
                                WriteColor("[Example] ", item);
                                System.Console.WriteLine(item.ToString());
                            }
                            return;
                        }
                    }
                    finally
                    {
                        i++;
                    }
                }
                else
                {
                    if (i<args.Length-1&&read_.IsMatch(args[i+1])==false)
                    {
                        value.Append(args[i]);
                        value.Append(" ");
                    }
                    else
                    {
                        value.Append(args[i]);
                    }
                }
            }
            WriteColor(value.ToString(), color);
        }

        /// <summary>
        /// 打印有颜色的内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        public static void WriteColor(string str, ConsoleColor color)
        {
            ConsoleColor currentForeColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.Write(str);
            System.Console.ForegroundColor = currentForeColor;
        }
    }

    /// <summary>
    /// 工程追加功能
    /// </summary>
    public static class ProgramAdd
    {
        /// <summary>
        /// 单词第一个字大写
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToUpperFirst(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            else
            {
                var ret = new StringBuilder();
                var first = input[0].ToString().ToUpper();
                ret.Append(first);
                for (int i = 1; i < input.Length; i++)
                {
                    ret.Append(input[i]);
                }
                return ret.ToString();
            }
        }
    }
}
