using ReptileUI.Properties;
using ReptileUI.Rule;
using ReptileUI.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReptileUI
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 资源文件
        /// </summary>
        ComponentResourceManager resource = new ComponentResourceManager(typeof(Resources));

        /// <summary>
        /// 正则存放块
        /// </summary>
        ReadRules readRules;

        public Form1()
        {
            InitializeComponent();

            //设置图标
            this.Icon = (Icon)resource.GetObject("favicon");

            //初始化INI读取模块
            Program.iniFile = new IniFileOperation2(Program.settingIni);
            
            //初始化python爬虫工具
            Program.pythonGet = new Python();
            //Program.pythonGet.SetEndText(Item.TempEnd);

            //读取INI
            this.textBox1.Text = Program.iniFile.Read(Program.uiSetting, "webUrl");
            this.textBox2.Text = Program.iniFile.Read(Program.uiSetting, "webUrlForEach");

            //设置初始导出文件位置
            this.textBox3.Text = Program.iniFile.Read(Program.uiSetting, "file", 
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\output.txt");
            this.textBox4.Text = Program.iniFile.Read(Program.uiSetting, "ini",
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\output.ini");

            //设置默认使用的正则表达式
            this.readRules = new ReadRules();
            this.comboBox1.DataSource = readRules.ReadDDs;
            try
            {
                this.comboBox1.SelectedIndex = 4;
            }
            catch (Exception)
            {
                //跳过
            }

            this.comboBox2.DataSource = readRules.ReadTexts;
            try
            {
                this.comboBox2.SelectedIndex = 6;
            }
            catch (Exception)
            {
                //跳过
            }

            this.comboBox3.DataSource = readRules.ReadLines;
            try
            {
                this.comboBox3.SelectedIndex = 1;
            }
            catch (Exception)
            {
                //跳过
            }

            //设置多行选择
            this.label8.Enabled = false;
            this.comboBox3.Enabled = false;
            this.button8.Enabled = false;

            //设置toolTip
            this.toolTip1.SetToolTip(label1, "爬取的小说的目录网址");
            this.toolTip1.SetToolTip(label2, "因为通常爬取获得的章节URL不包含http://www.xxx.com的头\n因此通过爬取的小说的目录网址截断获取,也可以自己编辑");
            this.toolTip1.SetToolTip(label3, "输出配置文件,用于保存爬虫进度");
            this.toolTip1.SetToolTip(label4, "输出文件,用于保存爬取内容");
            this.toolTip1.SetToolTip(label6, $"用于读取章节路径的正则表达式,\n当前正则:{comboBox1.Text}");
            this.toolTip1.SetToolTip(label7, $"用于读取整个章节的正则表达式,\n当前正则:{comboBox2.Text}");
            this.toolTip1.SetToolTip(label8, $"用于读取多行章节的正则表达式,\n当前正则:{comboBox3.Text}");

            this.toolTip1.SetToolTip(groupBox2, "爬取页面内容使用的正则表达式");
            this.toolTip1.SetToolTip(groupBox1, "爬取过程中需要的设置");
            this.toolTip1.SetToolTip(groupBox3, "使用的爬虫模块");

            this.toolTip1.SetToolTip(radioButton1, "使用Python的Urllib3模块爬取页面内容");
            this.toolTip1.SetToolTip(radioButton2, "使用ChromeDriver来爬取页面内容(尚未完成)");
            this.toolTip1.SetToolTip(radioButton3, "章节内容多以<div>标签包裹,换行通常使用<br/>");
            this.toolTip1.SetToolTip(radioButton4, "章节内容多以<p>标签包裹,每个段落前后都有<p>和</p>");

            this.toolTip1.SetToolTip(textBox1, $"{textBox1.Text}\n双击打开网站");
            this.toolTip1.SetToolTip(textBox2, textBox2.Text);
            this.toolTip1.SetToolTip(textBox3, textBox3.Text);
            this.toolTip1.SetToolTip(textBox4, textBox4.Text);

            this.toolTip1.SetToolTip(comboBox1, $"当前正则:{comboBox1.Text}");
            this.toolTip1.SetToolTip(comboBox2, $"当前正则:{comboBox2.Text}");
            this.toolTip1.SetToolTip(comboBox3, $"当前正则:{comboBox3.Text}");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Item.CMDStart();
        }

        private void Form1_Unload(object sender, FormClosedEventArgs e)
        {
            Item.CMDClose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var texts = textBox1.Text.Split('.');
            var part3 = texts[texts.Length - 1];
            var part3s = part3.Split('/','\\');

            textBox2.Text = texts.Take(texts.Length - 1).Join(".")+"."+part3s[0];

            Program.iniFile.Write(
                Program.uiSetting,
                "webUrl",
                textBox1.Text
            );
            Program.iniFile.Write(
                Program.uiSetting,
                "webUrlForEach",
                textBox2.Text
            );

            this.toolTip1.SetToolTip(textBox1, $"{textBox1.Text}\n双击打开网站");
        }

        private void textBox1_DoubleClick(object sender,EventArgs e)
        {
            Item.OpenOnWindows(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox2.ReadOnly = !this.textBox2.ReadOnly;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Program.iniFile.Write(
                Program.uiSetting,
                "webUrlForEach",
                textBox2.Text
            );

            this.toolTip1.SetToolTip(textBox2, textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string outputFilePath = this.textBox3.Text;
            var fileInfo = new FileInfo(outputFilePath);
            Item.ChoiceFolder(ref outputFilePath, "选择输出文件夹",fileInfo.DirectoryName);
            this.textBox3.Text = outputFilePath+"\\output.ini";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string outputFilePath = this.textBox4.Text;
            var fileInfo = new FileInfo(outputFilePath);
            Item.ChoiceFolder(ref outputFilePath, "选择输出文件夹", fileInfo.DirectoryName);
            this.textBox4.Text = outputFilePath + "\\output.txt";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.toolTip1.SetToolTip(label6, $"用于读取章节路径的正则表达式,\n当前正则:{comboBox1.Text}");
            this.toolTip1.SetToolTip(comboBox1, $"当前正则:{comboBox1.Text}");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.toolTip1.SetToolTip(label7, $"用于读取整个章节的正则表达式,\n当前正则:{comboBox2.Text}");
            this.toolTip1.SetToolTip(comboBox2, $"当前正则:{comboBox2.Text}");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            this.toolTip1.SetToolTip(textBox3, textBox3.Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            this.toolTip1.SetToolTip(textBox4, textBox4.Text);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.toolTip1.SetToolTip(label8, $"用于读取多行章节的正则表达式,\n当前正则:{comboBox3.Text}");
            this.toolTip1.SetToolTip(comboBox3, $"当前正则:{comboBox3.Text}");
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

            this.label7.Enabled = true;
            this.comboBox2.Enabled = true;
            this.button7.Enabled = true;

            this.label8.Enabled = false;
            this.comboBox3.Enabled = false;
            this.button8.Enabled = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

            this.label7.Enabled = false;
            this.comboBox2.Enabled = false;
            this.button7.Enabled = false;

            this.label8.Enabled = true;
            this.comboBox3.Enabled = true;
            this.button8.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var htmlValue = GetHtmlValue(textBox1.Text);
            RegexTest regexTest = new RegexTest("查看界面无法修改正则表达式",htmlValue);
            regexTest.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var htmlValue = GetHtmlValue(textBox1.Text);
            using (RegexTest dialog = new RegexTest(comboBox1.Text, htmlValue))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox1.Text = dialog.regexValue;
                    Item.Log(comboBox1.Text);
                    this.toolTip1.SetToolTip(comboBox1, $"用于读取章节路径的正则表达式,\n当前正则:{comboBox1.Text}");

                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            //初始化工作INI模块
            try
            {
                Program.workIni = new IniFileOperation2("output.ini");
            }
            catch (FileNotFoundException)
            {
                FileInfo file = new FileInfo("output.ini");
                var tempStream = file.Create();
                tempStream.Close();
                Program.workIni = new IniFileOperation2("output.ini");
                Program.workIni.Write("", Program.WEB_URL, "");
                Program.workIni.Write("", Program.WEB_URLS, "");
                Program.workIni.Write("", Program.WEB_NAMES, "");
                Program.workIni.Write("", Program.WEB_INDEX, "0");
                //保存
                Program.workIni.Save();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var htmlValue = GetHtmlValue(textBox1.Text);            
            var readChapter = Item.CreateRegex(comboBox1.Text);
            var chapters = readChapter.Matches(htmlValue);
            if (chapters.Count==0)
            {
                // 没读取到章节数据
                MessageBox.Show("读取章节数据错误","错误!",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var baseUrl = textBox2.Text.Replace('"', ' ').Trim();
            var options = new List<(string,string)>();
            //var readUrl = new Regex("^href=\"([^\"]*)\"$");
            var readUrl = new Regex("^/");
            var readTitle = new Regex("[\u4e00-\u9fa5]+");
            foreach (Match match in chapters) {
                var item = ("", "");
                for (int i = 0; i < match.Groups.Count; i++)
                {
                    if (readUrl.IsMatch(match.Groups[i].Value))
                    {
                        item.Item1 = baseUrl + match.Groups[i].Value;
                    }
                    else if (readTitle.IsMatch(match.Groups[i].Value))
                    {
                        item.Item2 = match.Groups[i].Value;
                    }
                }
                //Item.Log(item.Item1 + " " + item.Item2);
                options.Add(item);
            }

            using (Select select = new Select(SelectEnum.CHAPTER,options))
            {
                if (select.ShowDialog() == DialogResult.OK)
                {
                    var index = select.selectIndex;
                    if (index == -1)
                    {
                        MessageBox.Show("没有选中任何章节!","警告",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        var tuple = select.selectValue;
                        MessageBox.Show(tuple.Item1);
                        htmlValue = GetHtmlValue(tuple.Item1);

                        // 打开正则界面
                        using (RegexTest regexTest = new RegexTest(this.comboBox2.Text,htmlValue))
                        {
                            if (regexTest.ShowDialog() == DialogResult.OK)
                            {
                                this.comboBox2.Text = regexTest.regexValue;
                                Item.Log(comboBox2.Text);
                                this.toolTip1.SetToolTip(comboBox2, $"用于读取整个章节的正则表达式,\n当前正则:{comboBox2.Text}");
                            }
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var htmlValue = GetHtmlValue(textBox1.Text);
            var readChapter = Item.CreateRegex(comboBox1.Text);
            var chapters = readChapter.Matches(htmlValue);
            if (chapters.Count == 0)
            {
                // 没读取到章节数据
                MessageBox.Show("读取章节数据错误", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var baseUrl = textBox2.Text.Replace('"', ' ').Trim();
            var options = new List<(string, string)>();
            //var readUrl = new Regex("^href=\"([^\"]*)\"$");
            var readUrl = new Regex("^/");
            var readTitle = new Regex("[\u4e00-\u9fa5]+");
            foreach (Match match in chapters)
            {
                var item = ("", "");
                for (int i = 0; i < match.Groups.Count; i++)
                {
                    if (readUrl.IsMatch(match.Groups[i].Value))
                    {
                        item.Item1 = baseUrl + match.Groups[i].Value;
                    }
                    else if (readTitle.IsMatch(match.Groups[i].Value))
                    {
                        item.Item2 = match.Groups[i].Value;
                    }
                }
                //Item.Log(item.Item1 + " " + item.Item2);
                options.Add(item);
            }

            using (Select select = new Select(SelectEnum.CHAPTER, options))
            {
                if (select.ShowDialog() == DialogResult.OK)
                {
                    var index = select.selectIndex;
                    this.textBox5.Text = index.ToString();
                    Program.iniFile.Write("", Program.WEB_INDEX, index.ToString());
                }
            }
        }

        #region 功能模块

        /// <summary>
        /// 获取网页内容
        /// <br />
        /// 之后如果需要添加读取方式,修改此函数就行
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetHtmlValue(string url)
        {
            var htmlValue = "";
            if (radioButton1.Checked)
            {
                try
                {
                    htmlValue = Program.pythonGet.Get(url);
                    //Item.Log(htmlValue);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (radioButton2.Checked)
            {
                // 尚不支持ChromeDriver
                MessageBox.Show("尚不支持ChromeDriver", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return htmlValue;
        }

        #endregion
    }
}
