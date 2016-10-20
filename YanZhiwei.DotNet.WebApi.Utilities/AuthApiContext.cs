﻿namespace YanZhiwei.DotNet.WebApi.Utilities
{
    using System;
    
    using YanZhiwei.DotNet.WebApi.Utilities.Model;
    
    /// <summary>
    /// AuthApi配置上下文
    /// </summary>
    /// 时间：2016/10/20 13:58
    /// 备注：
    public class AuthApiContext
    {
        #region Constructors
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authApiService">IAuthApi</param>
        /// 时间：2016/10/20 16:23
        /// 备注：
        public AuthApiContext(IAuthApi authApiService)
        {
            AuthApiService = authApiService;
        }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// 时间：2016/10/20 16:23
        /// 备注：
        public AuthApiContext()
        : this(new JWTAuthService())
        {
        }
        
        #endregion Constructors
        
        #region Properties
        
        /// <summary>
        /// Web Api验证接口
        /// </summary>
        public IAuthApi AuthApiService
        {
            get;
            set;
        }
        
        /// <summary>
        /// WEBAPI验证配置项
        /// </summary>
        internal static WrapAuthApiConfigItem ConfigItem
        {
            get
            {
                WrapAuthApiConfigItem _item = new WrapAuthApiConfigItem();
                _item.TimspanExpiredMinutes = 5;
                _item.SharedKey = "yanzhiwei";
                _item.TokenExpiredDays = 1;
                return _item;
            }
        }
        
        #endregion Properties
        
        #region Methods
        
        /// <summary>
        /// 获取用户令牌
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="signature">加密签名字符串</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="appSecret">应用接入ID对应Key</param>
        /// <returns>用户令牌信息</returns>
        /// 时间：2016/10/20 16:04
        /// 备注：
        public TokenResult GetAccessToken(string userId, string signature, string timestamp, string nonce, string appSecret)
        {
            return AuthApiService.GetAccessToken(userId, signature, timestamp, nonce, appSecret, ConfigItem.SharedKey, ConfigItem.TimspanExpiredMinutes);
        }
        
        /// <summary>
        /// 检查用户令牌
        /// </summary>
        /// <param name="token">用户令牌</param>
        /// <returns>
        /// 检查结果
        /// </returns>
        public Tuple<bool, string> ValidateToken(string token)
        {
            return AuthApiService.ValidateToken(token, ConfigItem.SharedKey, ConfigItem.TokenExpiredDays);
        }
        
        #endregion Methods
    }
}