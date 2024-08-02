using ReptileUI.Properties;
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
            this.options.ForEach(item =>
            {
                this.listBox1.Items.Add(item.Item2);
            });
            if (this.optionIndex!=null)
            {
                this.listBox1.SelectedIndex = this.optionIndex.Value;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectIndex = this.listBox1.SelectedIndex;
            this.selectValue = this.options[selectIndex];
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

    /// <summary>
    /// 选择器分类
    /// </summary>
    public enum SelectEnum
    {
        /// <summary>
        /// 错误选择
        /// </summary>
        None = 0,
        /// <summary>
        /// 章节选择
        /// </summary>
        CHAPTER = 1,


    }

    /// <summary>
    /// 选择器分类追加类
    /// </summary>
    public static class SelectEnumAdd
    {
        /// <summary>
        /// 根据枚举获取选框标头
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetTitle(this SelectEnum @enum)
        {
            var ret = "错误选择";
            switch (@enum)
            {
                case SelectEnum.CHAPTER:
                    ret = "选择章节";
                    break;
                case SelectEnum.None:
                default:
                    break;
            }
            return ret;
        }
    }
}
