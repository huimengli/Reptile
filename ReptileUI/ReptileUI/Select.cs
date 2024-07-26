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
        /// 选中的对象
        /// </summary>
        public string selectValue;

        /// <summary>
        /// 选中的指针
        /// </summary>
        public int selectIndex;

        public Select(SelectEnum @enum,List<(string,string)> options)
        {
            InitializeComponent();

            //设置参数
            this.selectEnum = @enum;
            this.selectIndex = 0;
            this.options = options;

            //设置选择器标头
            this.Text = @enum.GetTitle();

            //设置图标
            this.Icon = (Icon)resource.GetObject("favicon");
        }

        private void Select_Load(object sender, EventArgs e)
        {

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
