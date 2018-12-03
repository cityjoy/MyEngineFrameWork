
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Engine.Domain.Entity;
using Engine.Application.Model;
using Engine.Infrastructure.Utils;
namespace Engine.Application
{
    /// <summary>
    /// 用户服务接口
    /// </summary>

    public interface IUserServer : IServer<User>
    {
        #region 新版JsonWebToken
        /// <summary>
        /// 获取登录用户的JsonWebToken令牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        UserAccessToken GetJsonWebToken(LoginViewModel model);

        /// <summary>
        /// 从JsonWebToken获取用会话户详情
        /// </summary>
        /// <param name="status">获取状态(0:无效令牌,-1:错误令牌,-2:过期(需刷新),-3:需要重新授权,-4:密码已更改,
        /// -5:系统服务已被停用,-6:系统服务已到期)</param>
        /// <param name="authAccessToken">令牌</param>
        /// <param name="newAccessToken">刷新后的令牌</param>
        /// <returns>登录用户</returns>
        User GetSessionUserFromJsonWebToken(string authAccessToken, out int status, out UserAccessToken newAccessToken);
    
        #endregion

        #region 旧版
        /// <summary>
        /// 获取登录用户的授权访问令牌
        /// </summary>
        ///<param name="model">用户登录视图模型</param>
        /// <returns>授权令牌。结果错误码(-1:用户名或密码错误,-2:用户不存在,-3:用户被禁用,-400:账号不能为空,-401:密码不能为空,-500:程序内部错误)</returns>
        UserAccessToken GrantUserAccessToken(LoginViewModel model);

            /// <summary>
        /// 使用授权令牌获取对应的用户数据
        /// </summary>
        /// <param name="accessToken">授权令牌</param>
        User GetUserByAccessToken(string accessToken);

        /// <summary>
        /// 刷新令牌，根据原令牌刷新获取新的令牌
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>授权令牌。结果错误码(0:出错,-1:错误令牌,-3:令牌过期,-4:密码已变更)</returns>
        UserAccessToken RefreshUserAccessToken(string accessToken);

         /// <summary>
        /// 应用层从Cookie的AccessToken获取用户详情
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="status">获取状态(-1:错误令牌,-2:过期(需刷新),-3:需要重新授权,-4:密码已更改,
        /// 0:无效令牌,-5:系统服务已被停用,-6:系统服务已到期)</param>
        /// <param name="authAccessToken">返回令牌</param>
        /// <returns>登录用户</returns>
        User GetUserFromAccessTokenCookie(WebSession session, string authAccessToken, out int status );
        #endregion
    }
}
