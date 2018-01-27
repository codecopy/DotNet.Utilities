﻿namespace YanZhiwei.DotNet2.Utilities.Core
{
    using System.Threading;

    /// <summary>
    /// 计数器
    /// </summary>
    public sealed class Counter
    {
        #region Fields

        private long current = 0;

        #endregion Fields

        #region Methods

        /// <summary>
        /// 获取当前数值
        /// </summary>
        /// <returns>当前数值</returns>
        public long GetValue()
        {
            return Interlocked.Read(ref current);
        }

        /// <summary>
        /// 加一
        /// </summary>
        /// <returns>当前数值</returns>
        public long NextValue()
        {
            return Interlocked.Increment(ref this.current);
        }

        /// <summary>
        /// 重置为0
        /// </summary>
        public void Reset()
        {
            Reset(0);
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="resetValue">重置数值</param>
        public void Reset(long resetValue)
        {
            Interlocked.Exchange(ref current, resetValue);
        }

        #endregion Methods
    }
}