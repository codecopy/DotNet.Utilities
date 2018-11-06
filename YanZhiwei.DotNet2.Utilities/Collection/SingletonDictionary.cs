﻿namespace YanZhiwei.DotNet2.Utilities.Collection
{
    using Core;
    using System.Collections.Generic;

    /// <summary>
    /// 创建一个单例字典，该实例的生命周期将跟随整个应用程序
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    public class SingletonDictionary<TKey, TValue> : SingletonType<IDictionary<TKey, TValue>>
    {
        #region Constructors

        static SingletonDictionary()
        {
            SingletonType<IDictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 获取指定类型的字典的单例实例
        /// </summary>
        public new static IDictionary<TKey, TValue> Instance => SingletonType<IDictionary<TKey, TValue>>.Instance;

        #endregion Properties
    }
}