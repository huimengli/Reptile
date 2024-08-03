using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReptileUI.Enums
{
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
