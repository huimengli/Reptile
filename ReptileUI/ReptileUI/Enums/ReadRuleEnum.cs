using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReptileUI.Enums
{
    /// <summary>
    /// 读取规则枚举类
    /// </summary>
    public enum ReadRuleEnum
    {
        /// <summary>
        /// 错误
        /// </summary>
        NONE=0,

        /// <summary>
        /// 读取章节的正则
        /// </summary>
        READ_DD=1,

        /// <summary>
        /// 读取大段内容的正则
        /// </summary>
        READ_TEXT=2,

        /// <summary>
        /// 读取多行内容的正则
        /// </summary>
        READ_LINE=3,
    }
}
