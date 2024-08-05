using ReptileUI.Enums;
using ReptileUI.Properties;
using ReptileUI.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReptileUI
{
    public partial class Select : Form
    {
        /// <summary>
        /// 资源文件
        /// </summary>
        private ComponentResourceManager resource = new ComponentResourceManager(typeof(Resources));

        /// <summary>
        /// 选择模式
        /// </summary>
        private SelectEnum selectEnum;

        /// <summary>
        /// 所有选项 (url:章节名)
        /// </summary>
        private List<(string, string)> options;

        /// <summary>
        /// 初始化指针
        /// </summary>
        private int? optionIndex;

        /// <summary>
        /// 测试用正则
        /// </summary>
        public string testRegex;

        /// <summary>
        /// 选中的对象
        /// </summary>
        public (string,string) selectValue;

        /// <summary>
        /// 选中的指针
        /// </summary>
        public int selectIndex = 0;

        public Select(SelectEnum @enum,List<(string,string)> options)
        {
            InitializeComponent();

            //设置参数
            this.selectEnum = @enum;
            this.selectIndex = 0;
            this.options = options;
            this.optionIndex = null;
            this.testRegex = "";

            //设置选择器标头
            this.Text = @enum.GetTitle();

            //设置图标
            this.Icon = (Icon)resource.GetObject("favicon");
        }

        public Select(SelectEnum @enum,List<(string,string)> options,int optionIndex):this(@enum,options)
        {
            //设置参数
            this.optionIndex=optionIndex;
        }

        public Select(SelectEnum @enum,List<(string,string)> options,string testRegex):this(@enum,options)
        {
            //设置参数
            this.testRegex=testRegex;
        }

        private void Select_Load(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            this.listBox1.SelectedItems.Clear();
            switch (this.selectEnum)
            {
                case SelectEnum.CHAPTER:
                    this.options.ForEach(item =>
                    {
                        this.listBox1.Items.Add(item.Item2);
                    });
                    if (this.optionIndex != null)
                    {
                        this.listBox1.SelectedIndex = this.optionIndex.Value;
                    }
                    break;
                case SelectEnum.CHAPTER_REVERSE:
                    this.options.ReverseThis().ForEach(item =>
                    {
                        this.listBox1.Items.Add(item.Item2);
                    });
                    if (this.optionIndex != null)
                    {
                        this.listBox1.SelectedIndex = this.options.Count - this.optionIndex.Value;
                    }
                    break;
                case SelectEnum.None:
                default:
                    break;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectIndex = this.listBox1.SelectedIndex;
            switch (this.selectEnum)
            {
                case SelectEnum.CHAPTER:
                    this.selectValue = this.options[selectIndex];
                    break;
                case SelectEnum.CHAPTER_REVERSE:
                    this.selectValue = this.options[selectIndex];
                    this.selectIndex = this.options.Count - this.selectIndex;
                    break;
                case SelectEnum.None:
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult=DialogResult.OK;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex<0)
            {
                MessageBox.Show("没有选中章节!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

            }
        }
    }
}
