using ReptileUI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReptileUI.Class
{
    /// <summary>
    /// 增强字典类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryEX<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public DictionaryEX() { }

        public DictionaryEX(Dictionary<TKey, TValue> dict)
        {
            foreach (var item in dict)
            {
                this.Add(item.Key, item.Value);
            }
        }

        public DictionaryEX(TKey[] keys, TValue[] values)
        {
            for (int i = 0; i < Math.Min(keys.Length, values.Length); i++)
            {
                this.Add(keys[i], values[i]);
            }
        }

        public DictionaryEX(List<TKey> keys, List<TValue> values) : this(keys.ToArray(), values.ToArray())
        {

        }

        //
        // 摘要:
        //     获取或设置与指定的键关联的值。
        //
        // 参数:
        //   key:
        //     要获取或设置的值的键。
        //
        // 返回结果:
        //     与指定的键相关联的值。 如果指定键未找到，则 Get 操作引发 System.Collections.Generic.KeyNotFoundException，而
        //     Set 操作创建一个带指定键的新元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     key 为 null。
        /// <summary>
        /// 获取或设置与指定的键关联的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new TValue this[TKey key]
        {
            set
            {

            }
            get
            {
                try
                {
                    return base[key];
                }
                catch (KeyNotFoundException)
                {
                    return default(TValue);
                }
            }
        }
    }

    /// <summary>
    /// 本地化存储追加类
    /// </summary>
    public static class DictionaryEXAdd
    {
        #region 保存

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this DictionaryEX<string, bool> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(item.Value ? 1 : 0);
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this DictionaryEX<string, int> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(item.Value);
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this DictionaryEX<string, string> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(Base64.Encode(item.Value));
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this Dictionary<string, bool> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(item.Value ? 1 : 0);
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this Dictionary<string, int> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(item.Value);
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        /// <summary>
        /// 重写转为字符串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToSave(this Dictionary<string, string> dict)
        {
            var ret = new StringBuilder();
            ret.Append("{");
            foreach (var item in dict)
            {
                ret.Append(Base64.Encode(item.Key));
                ret.Append(':');
                ret.Append(Base64.Encode(item.Value));
                ret.Append(",");
            }
            ret.Append("}");
            return ret.ToString();
        }

        #endregion

        #region 读取

        public static Dictionary<string, bool> FromBoolDict(this string str)
        {
            var ret = new Dictionary<string, bool>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            ; string key;
            bool val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = value[1] == "1" ? true : false;
                ret.Add(key, val);
            }

            return ret;
        }

        public static Dictionary<string, int> FromIntDict(this string str)
        {
            var ret = new Dictionary<string, int>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            string key;
            int val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = int.Parse(value[1]);
                ret.Add(key, val);
            }

            return ret;
        }

        public static Dictionary<string, string> FromStringDict(this string str)
        {
            var ret = new Dictionary<string, string>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            string key;
            string val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = Base64.Decode(value[1]);
                ret.Add(key, val);
            }

            return ret;
        }

        public static DictionaryEX<string, bool> FromBoolDictEX(this string str)
        {
            var ret = new DictionaryEX<string, bool>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            ; string key;
            bool val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = value[1] == "1" ? true : false;
                ret.Add(key, val);
            }

            return ret;
        }

        public static DictionaryEX<string, int> FromIntDictEX(this string str)
        {
            var ret = new DictionaryEX<string, int>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            string key;
            int val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = int.Parse(value[1]);
                ret.Add(key, val);
            }

            return ret;
        }

        public static DictionaryEX<string, string> FromStringDictEX(this string str)
        {
            var ret = new DictionaryEX<string, string>();
            str = str.Replace("{", "").Replace("}", "");
            var values = str.Split(',');
            string[] value;
            string key;
            string val;

            for (int i = 0; i < values.Length; i++)
            {
                value = values[i].Split(':');
                if (value.Length == 1)
                {
                    continue;
                }
                key = Base64.Decode(value[0]);
                val = Base64.Decode(value[1]);
                ret.Add(key, val);
            }

            return ret;
        }

        #endregion

        #region 重载ToString

        /// <summary>
        /// 重载ToString
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString<TKey, TValue>(this Dictionary<TKey, TValue> dict, bool tf)
        {
            var ret = new StringBuilder();
            ret.Append("Dictionary<");
            ret.Append(typeof(TKey));
            ret.Append(", ");
            ret.Append(typeof(TValue));
            ret.Append(">(");
            ret.Append(dict.Count);
            ret.Append(") { ");
            foreach (var item in dict)
            {
                ret.Append("{ ");
                ret.Append("\"");
                ret.Append(item.Key.ToString());
                ret.Append("\", \"");
                ret.Append(item.Value.ToString());
                ret.Append("\" }, ");
            }
            ret.Append("}");
            return ret.ToString();
        }

        #endregion

        #region ToDictionaryEX
        /// <summary>
        /// 添加了转为增强型字典的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="ts"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static DictionaryEX<R, T> ToDictionaryEX<T, R>(this List<T> ts, Func<T, R> getKey)
        {
            var ret = new DictionaryEX<R, T>();

            ts.ForEach(t =>
            {
                ret.Add(getKey.Invoke(t), t);
            });

            return ret;
        }

        /// <summary>
        /// 添加了转为增强型字典的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="ts"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static DictionaryEX<R, T> ToDictionaryEX<T, R>(this List<T> ts, Func<T, int, R> getKey)
        {
            var ret = new DictionaryEX<R, T>();

            ts.ForEach((t, i) =>
            {
                ret.Add(getKey(t, i), t);
            });

            return ret;
        }
        #endregion

        #region 通用遍历功能
        /// <summary>
        /// 将字典中的每个键值对进行遍历操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        public static void ForEach<TKey, TValue>(
            this DictionaryEX<TKey, TValue> dict,
            Action<TKey, TValue> action
        )
        {
            foreach (var item in dict)
            {
                action.Invoke(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 将字典中的每个键值对进行遍历操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="action"></param>
        public static void ForEach<TKey, TValue>(
            this DictionaryEX<TKey, TValue> dict,
            Action<TKey, TValue, int> action
        )
        {
            var index = 0;
            foreach (var item in dict)
            {
                action.Invoke(item.Key, item.Value, index++);
            }
        }

        #endregion

        #region 遍历功能(KeyValuePair)

        /// <summary>
        /// 将字典中的每个键值对进行遍历操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="action"></param>
        public static void ForEach<TKey,TValue>(
            this DictionaryEX<TKey,TValue> dict,
            Action<KeyValuePair<TKey,TValue>> action
        ){
            foreach (var item in dict)
            {
                action.Invoke(item);
            }
        }

        /// <summary>
        /// 将字典中的每个键对值进行操作并转换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="RKey"></typeparam>
        /// <typeparam name="RValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static DictionaryEX<RKey, RValue> Map<TKey, TValue, RKey, RValue>(
            this DictionaryEX<TKey, TValue> dict,
            Func<KeyValuePair<TKey,TValue>,KeyValuePair<RKey,RValue>> func
        ){
            var ret = new DictionaryEX<RKey, RValue>();
            dict.ForEach(item =>
            {
                var itemRet = func(item);
                ret.Add(itemRet.Key, itemRet.Value);
            });
            return ret;
        }

        /// <summary>
        /// 将字典中的每个键对值进行操作并转换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="RKey"></typeparam>
        /// <typeparam name="RValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static DictionaryEX<RKey,RValue> Map<TKey,TValue,RKey,RValue>(
            this DictionaryEX<TKey,TValue> dict,
            Func<TKey,TValue,KeyValuePair<RKey,RValue>> func
        ){
            var ret = new DictionaryEX<RKey, RValue>();
            dict.ForEach((key, value) =>
            {
                var itemRet = func(key, value);
                ret.Add(itemRet.Key, itemRet.Value);
            });
            return ret;
        }

        /// <summary>
        /// 将字典中的每个键对值进行操作并转换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="RKey"></typeparam>
        /// <typeparam name="RValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static DictionaryEX<RKey,RValue> Map<TKey,TValue,RKey,RValue>(
            this DictionaryEX<TKey,TValue> dict,
            Func<TKey,TValue,int,KeyValuePair<RKey,RValue>> func
        ){
            var ret = new DictionaryEX<RKey, RValue>();
            dict.ForEach((key, value, index) =>
            {
                var itemRet = func(key, value, index);
                ret.Add(itemRet.Key, itemRet.Value);
            });
            return ret;
        }
        #endregion

        #region 遍历功能(Tuple)

        /// <summary>
        /// 将字典中的每个键值对进行遍历操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="action"></param>
        public static void ForEach<TKey,TValue>(
            this DictionaryEX<TKey,TValue> dict,
            Action<(TKey,TValue)> action
        ){
            foreach (var item in dict)
            {
                action.Invoke((item.Key, item.Value));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="RKey"></typeparam>
        /// <typeparam name="RValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static DictionaryEX<RKey,RValue> Map<TKey,TValue,RKey,RValue>(
            this DictionaryEX<TKey,TValue> dict,
            Func<(TKey,TValue),(RKey,RValue)> func
        )
        {
            var ret = new DictionaryEX<RKey, RValue>();
            dict.ForEach(item =>
            {
                var itemRet = func(item);
                ret.Add(itemRet.Item1, itemRet.Item2);
            });
            return ret;
        }

        /// <summary>
        /// 将字典中的每个键对值进行操作并转换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="RKey"></typeparam>
        /// <typeparam name="RValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static DictionaryEX<RKey,RValue> Map<TKey,TValue,RKey,RValue>(
            this DictionaryEX<TKey,TValue> dict,
            Func<TKey,TValue,(RKey,RValue)> func
        )
        {
            var ret = new DictionaryEX<RKey, RValue>();
            dict.ForEach((key, value) =>
            {
                var itemRet = func(key, value);
                ret.Add(itemRet.Item1, itemRet.Item2);
            });
            return ret;
        }

        /// <summary>
        /// 将字典中的每个键对值进行操作并转换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="RKey"></typeparam>
        /// <typeparam name="RValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static DictionaryEX<RKey,RValue> Map<TKey,TValue,RKey,RValue>(
            this DictionaryEX<TKey,TValue> dict,
            Func<TKey,TValue,int,(RKey,RValue)> func
        )
        {
            var ret = new DictionaryEX<RKey, RValue>();
            dict.ForEach((key, value, index) =>
            {
                var itemRet = func(key, value, index);
                ret.Add(itemRet.Item1, itemRet.Item2);
            });
            return ret;
        }

        #endregion
    }
}
