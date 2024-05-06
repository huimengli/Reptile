using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReptileUI.Tools
{
    /// <summary>
    /// 别用
    /// 本来想通过此类来重写||操作符的,失败了
    /// </summary>
    [Obsolete]
    class MyString
    {
        private static Dictionary<string, MyString> dic = new Dictionary<string, MyString>();
        string s;
        public MyString(string s)
        {
            this.s = s;
            dic[s] = this;
        }

        public static implicit operator MyString(string value)
        {
            if (dic.ContainsKey(value))
            {
                return dic[value];
            }
            return new MyString(value);
        }


        public static bool operator ==(MyString s1, MyString s2)
        {
            return s1.ToString() == s2.ToString();
        }

        public static bool operator !=(MyString s1, MyString s2)
        {
            return !(s1.ToString() == s2.ToString());
        }

        public override string ToString()
        {
            return s;
        }
    }
}
