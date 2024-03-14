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
    public partial class Form1 : Form
    {
        /// <summary>
        /// 资源文件
        /// </summary>
        ComponentResourceManager resource = new ComponentResourceManager(typeof(Resources));

        public Form1()
        {
            InitializeComponent();

            //设置图标
            this.Icon = (Icon)resource.GetObject("favicon");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Item.CMDStart();
        }

        private void Form1_Unload(object sender, FormClosedEventArgs e)
        {
            Item.CMDClose();
        }
    }
}
