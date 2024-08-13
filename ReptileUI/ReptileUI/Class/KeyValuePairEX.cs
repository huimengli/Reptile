using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReptileUI.Class
{
    //
    // 摘要:
    //     定义可设置或检索的键/值对。
    //
    // 类型参数:
    //   TKey:
    //     键的类型。
    //
    //   TValue:
    //     值的类型。
    public class KeyValuePairEX<TKey, TValue>
    {
        public KeyValuePairEX()
        {
            Key = default(TKey);
            Value = default(TValue);
        }
        //
        // 摘要:
        //     新实例初始化 System.Collections.Generic.KeyValuePair`2 具有指定的键和值结构。
        //
        // 参数:
        //   key:
        //     每个键/值对中定义的对象。
        //
        //   value:
        //     与关联的定义 key。
        public KeyValuePairEX(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public KeyValuePairEX(KeyValuePair<TKey,TValue> keyValuePair)
        {
            Key = keyValuePair.Key;
            Value = keyValuePair.Value;
        }

        //
        // 摘要:
        //     获取键/值对中的键。
        //
        // 返回结果:
        //     一个 TKey ，它表示的键 System.Collections.Generic.KeyValuePair`2。
        public TKey Key { get; }
        //
        // 摘要:
        //     获取键/值对中的值。
        //
        // 返回结果:
        //     一个 TValue 的值 System.Collections.Generic.KeyValuePair`2。
        public TValue Value { get; }

        //
        // 摘要:
        //     返回的字符串表示形式 System.Collections.Generic.KeyValuePair`2, ，使用的字符串表示形式的键和值。
        //
        // 返回结果:
        //     字符串表示形式 System.Collections.Generic.KeyValuePair`2, ，其中包括键和值的字符串表示形式。
        public override string ToString()
        {
            return $"{Key.ToString()}:{Value.ToString()}";
        }

        /// <summary>
        /// 转为原有的KeyValuePair
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<TKey,TValue> ToKeyValuePair()
        {
            return new KeyValuePair<TKey, TValue>(Key, Value);
        }

        /// <summary>
        /// 转为元组
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="keyValuePair"></param>
        /// <returns></returns>
        public (TKey, TValue) ToTuple()
        {
            return (Key, Value);
        }

        /// <summary>
        /// 从元组转化
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static KeyValuePair<TKey, TValue> Of((TKey, TValue) tuple)
        {
            return new KeyValuePair<TKey, TValue>(tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// 从KeyValue转化
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static KeyValuePair<TKey,TValue> Of(TKey key,TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    /// <summary>
    /// 追加功能
    /// </summary>
    public static class KeyValuePairAdd
    {
        #region KeyValuePair模块追加功能

        ///// <summary>
        ///// 将元组析构成KeyValuePair<>
        ///// </summary>
        ///// <typeparam name="TKey"></typeparam>
        ///// <typeparam name="TValue"></typeparam>
        ///// <param name="kvp"></param>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        //public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        //{
        //    key = kvp.Key;
        //    value = kvp.Value;
        //}


        #endregion
    }

    /// <summary>
    /// 为了简化名称
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KVPE<TKey, TValue> : KeyValuePairEX<TKey, TValue>
    {
        public KVPE():base()
        {

        }

        public KVPE(TKey key, TValue value) : base(key, value)
        {

        }

        public KVPE(KeyValuePair<TKey,TValue> pair) : base(pair)
        {

        }
    }
}
