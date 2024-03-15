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

            //读取INI
            Program.iniFile = new IniFileOperation(Program.settingIni);
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
            this.comboBox1.Text = readRules.GetReadDD(4).ToString();

            //设置toolTip
            this.toolTip1.SetToolTip(label1, "爬取的小说的目录网址");
            this.toolTip1.SetToolTip(label2, "因为通常爬取获得的章节URL不包含http://www.xxx.com/的头\n因此通过爬取的小说的目录网址截断获取,也可以自己编辑");

            this.toolTip1.SetToolTip(this.textBox1, textBox1.Text);
            this.toolTip1.SetToolTip(this.textBox2, textBox2.Text);
            this.toolTip1.SetToolTip(this.textBox3, textBox3.Text);
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string outputFilePath = this.textBox3.Text;
            var fileInfo = new FileInfo(outputFilePath);
            Item.ChoiceFolder(ref outputFilePath, "选择输出文件夹",fileInfo.DirectoryName);
            this.textBox3.Text = outputFilePath+"\\output.txt";
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

        }
    }
}
