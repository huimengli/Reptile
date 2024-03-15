using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReptileUI.Enums
{
    /// <summary>
    /// 编码加密方式
    /// </summary>
    public enum CodingMode
    {
        /// <summary>
        /// 没有加密
        /// </summary>
        NoCoding,

        /// <summary>
        /// SHA256加密
        /// </summary>
        SHA256,

        /// <summary>
        /// 双层MD5加密
        /// </summary>
        MD5,
    }
}
