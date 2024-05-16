//#define TEST_VALUE

using ReptileUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReptileUI.Tools;
using ReptileUI.TestValue;

namespace ReptileUI
{
    public partial class RegexTest : Form
    {

        /// <summary>
        /// 资源文件
        /// </summary>
        private ComponentResourceManager resource = new ComponentResourceManager(typeof(Resources));

        /// <summary>
        /// 文章内容
        /// </summary>
        private string textValue;

        /// <summary>
        /// 文章基础内容
        /// </summary>
        private string baseValue;

        /// <summary>
        /// 文章内容
        /// </summary>
        public string TextValue
        {
            get
            {
                return VisualizeInvisibles(textValue);
            }
            set
            {
                this.textValue = value;
                if (String.IsNullOrWhiteSpace(this.baseValue))
                {
                    this.baseValue = value;
                }
            }
        }

        /// <summary>
        /// 文章内容,备份用
        /// </summary>
        public string BaseValue
        {
            get
            {
                return baseValue;
            }
            //set
            //{
            //    this.baseValue = value;
            //}
        }

        /// <summary>
        /// 正则内容
        /// </summary>
        public string regexValue;

        /// <summary>
        /// 正则基础内容
        /// </summary>
        private string baseRegex;

        /// <summary>
        /// 正则内容,备份用
        /// </summary>
        public string BaseRegex
        {
            get
            {
                return baseRegex;
            }
        }

        /// <summary>
        /// 读取用的正则
        /// </summary>
        private Regex regex;

        /// <summary>
        /// 正则读取结果
        /// </summary>
        private MatchCollection matches;

        /// <summary>
        /// 正则读取的结果
        /// </summary>
        public MatchCollection Matches
        {
            get
            {
                return matches;
            }
            set
            {
                this.matches = value;
                this.matchesIndex = 0;
            }
        }

        /// <summary>
        /// 正则读取指针
        /// </summary>
        private int matchesIndex = 0;

        /// <summary>
        /// 颜色列表
        /// </summary>
        private List<Color> colors = new List<Color>
        {
            Color.Red,
            Color.Orange,
            //Color.Yellow, //这颜色太亮了
            Color.Green,
            Color.Blue,
            Color.Indigo,
            Color.Purple,
            Color.Gray,
            Color.DarkRed,
            Color.DarkOrange,
            Color.DarkGoldenrod,
            Color.DarkGreen,
            Color.DarkBlue,
        };

        public RegexTest(string regexValue) : this()
        {
            this.regexValue = regexValue;
            this.baseRegex = regexValue;
        }

        public RegexTest(string regexValue, string textValue) : this(regexValue)
        {
            this.TextValue = textValue;
        }

        public RegexTest()
        {
            InitializeComponent();

            //设置初始参数
            this.TextValue = "";
            this.regexValue = "";

#if TEST_VALUE
            this.TextValue = StaticValues.testChapter;

#endif

            //设置图标
            this.Icon = (Icon)resource.GetObject("favicon");
        }

        private void RegexTest_Load(object sender, EventArgs e)
        {
            this.richTextBox1.Text = this.TextValue;
            this.richTextBox2.Text = this.regexValue;
            //this.regex = new Regex(this.regexValue);
            this.regex = Item.CreateRegex(this.regexValue);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Focus();
            TestRegex();
        }

        private void TestRegex()
        {
            try
            {
                this.regex = Item.CreateRegex(this.regexValue);
            }
            catch (Exception err)
            {
                this.regex = null;
                MessageBox.Show($"正则表达式错误!\n{err.Message}", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (regex == null)
            {
                this.richTextBox1.Text = this.TextValue;
            }
            else
            {
                Matches = regex.Matches(this.TextValue);

                if (Matches != null && Matches.Count > 0)
                {
                    // 暂停布局，防止重绘和闪烁
                    richTextBox1.SuspendLayout();
                    richTextBox1.Text = this.TextValue;  // 只设置一次文本

                    richTextBox1.Select(0, 0);  // 确保开始选择前没有选中的文本

                    foreach (Match match in Matches)
                    {
                        for (int j = 0; j < Math.Min(match.Groups.Count, colors.Count); j++)
                        {
                            var group = match.Groups[j];
                            richTextBox1.Select(group.Index, group.Length);
                            richTextBox1.SelectionColor = colors[j];
                        }
                    }

                    // 恢复布局
                    richTextBox1.ResumeLayout();

                    // 转到第一个匹配
                    richTextBox1.Focus();
                    var g = Matches[0].Groups[0];
                    richTextBox1.Select(g.Index, g.Length);
                }
                else
                {
                    MessageBox.Show($"正则表达式错误\n无法匹配任何值!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 将不可见字符串可视化
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string VisualizeInvisibles(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsWhiteSpace(c))
                {
                    sb.Append(c == '\n' ? "\\n" : c == '\r' ? "\\r" : c == '\t' ? "\\t" : " ");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            this.regexValue = this.richTextBox2.Text;
            ////this.regex = new Regex(this.regexValue);
            //this.regex = Item.CreateRegex(this.regexValue);
        }

        private void 清空内容ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TextValue = "";
            this.richTextBox1.Text = "";
        }

        private void 清空正则ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.regexValue = "";
            this.richTextBox2.Text = "";
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.TextValue = "";
            this.richTextBox1.Text = "";

            this.regexValue = "";
            this.richTextBox2.Text = "";
        }

        private void 重置文本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TextValue = this.BaseValue;
            this.richTextBox1.Text = this.TextValue;
        }

        private void 重置正则ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.regexValue = this.BaseRegex;
            this.regex = Item.CreateRegex(this.regexValue);
            this.richTextBox2.Text = this.regexValue;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.TextValue = this.BaseValue;
            this.richTextBox1.Text = this.TextValue;

            this.regexValue = this.BaseRegex;
            this.regex = Item.CreateRegex(this.regexValue);
            this.richTextBox2.Text = this.regexValue;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 测试正则ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //button1_Click(sender, e);
            button1.Focus();
            TestRegex();
        }

        private void 下一个匹配ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Matches != null && Matches.Count > 0)
            {
                matchesIndex = (matchesIndex + 1) % Matches.Count;
                var match = Matches[matchesIndex];
                this.richTextBox1.Focus();
                this.richTextBox1.Select(match.Index, match.Length);
                //this.richTextBox1.ScrollToCaret(); // 确保选中的文本可见
            }
            else
            {
                MessageBox.Show("没有正则测试结果!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 上一个匹配ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Matches != null && Matches.Count > 0)
            {
                matchesIndex = (matchesIndex - 1 + Matches.Count) % Matches.Count;
                var match = Matches[matchesIndex];
                this.richTextBox1.Focus();
                this.richTextBox1.Select(match.Index, match.Length);
                //this.richTextBox1.ScrollToCaret(); // 确保选中的文本可见
            }
            else
            {
                MessageBox.Show("没有正则测试结果!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
