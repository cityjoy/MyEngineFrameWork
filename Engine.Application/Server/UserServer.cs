
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Engine.Domain.Entity;
using Engine.Domain.IRepository;
using Engine.Infrastructure.Data;
using Engine.Infrastructure.Utils;
using System.Web;
using Engine.Application.Model;
using Engine.Doman.Cache;
using Engine.Domain.Model;
using System.Configuration;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
namespace Engine.Application
{
    /// <summary>
    /// 用户服务实现
    /// </summary>

    public class UserServer : BaseServer<User>, IUserServer
    {


        public UserServer(IRepository<User> repository, IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }

        

        #region 旧版


        /// <summary>
        /// 构造授权用户访问令牌
        /// </summary>
        /// <param name="user">授权用户</param>
        private string CreateAuthUserAccessToken(User user)
        {
            long nowTimestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);

            string token = BuildAuthUserAccessToken(user.Id, user.Password, nowTimestamp, nowTimestamp, MathHelper.GetRandomInt(10000, 99999).ToString());

            return token;
        }

        /// <summary>
        /// 构造授权用户访问令牌
        /// </summary>
        private string BuildAuthUserAccessToken(int userId, string password, long freshTimestamp, long createdTimestamp, string nonce)
        {
            //{用户ID}|{用户类型}|{用户密码(已加密)}|{刷新时间}|{初始时间}|{随机数字}
            string str = string.Concat(userId, "|", password, "|", freshTimestamp, "|", createdTimestamp, "|", nonce);
            string encryptStr = SecurityHelper.DESCryptoEncode(str);

            return encryptStr;
        }

        /// <summary>
        /// 从授权访问令牌获取授权用户Id
        /// </summary>
        /// <param name="accessToken">授权访问令牌</param>
        /// <param name="user">用户精简信息</param>
        /// <returns>-1:错误令牌,-2:过期(需刷新),-3:需要重新授权,-4:密码已更改,0:无令牌,大于0:用户ID</returns>
        private int GetAuthUserFromAccessToken(string accessToken, out User user)
        {
            user = null;

            if (string.IsNullOrEmpty(accessToken))
            {
                return 0;
            }
            try
            {
                string decryptStr = SecurityHelper.DESCryptoDecode(accessToken);
                string[] info = decryptStr.Split('|');
                if (info != null && info.Length >= 5)
                {
                    int userId = ConvertHelper.ToInt32(info[0], 0);
                    string password = info[1];
                    long refreshTimestamp = ConvertHelper.ToInt64(info[2], 0);
                    long createdTimestamp = ConvertHelper.ToInt64(info[3], 0);
                    string rand = info[4];

                    long nowTimestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);
                    if (nowTimestamp - refreshTimestamp > 600) //过期，需要刷新
                    {
                        return -2;
                    }
                    if (nowTimestamp - createdTimestamp > 2592000) //超过30天，需要重新授权
                    {
                        return -3;
                    }

                    user = new User();
                    user.Id = userId;
                    user.Password = password;
                    return userId;
                }
                else
                {
                    return -1; //错误令牌，需要重新授权
                }
            }
            catch
            {
                return -1; //错误令牌，需要重新授权
            }
        }

        /// <summary>
        /// 获取登录用户的授权访问令牌
        /// (POST方式)
        /// </summary>
        ///<param name="model">用户登录视图模型</param>
        /// <returns>授权令牌。结果错误码(-1:用户名或密码错误,-2:用户不存在,-3:用户被禁用,-400:账号不能为空,-401:密码不能为空,-500:程序内部错误)</returns>
        public UserAccessToken GrantUserAccessToken(LoginViewModel model)
        {
            UserAccessToken tokenResult = new UserAccessToken();

            try
            {
                #region 数据检查

                if (string.IsNullOrEmpty(model.LoginAccount))
                {
                    tokenResult.Result = -400;
                    tokenResult.Message = "账号不能为空";

                    return tokenResult;
                }
                if (string.IsNullOrEmpty(model.LoginPassword))
                {
                    tokenResult.Result = -401;
                    tokenResult.Message = "密码不能为空";

                    return tokenResult;
                }

                #endregion

                User user = repository.GetSingle(m => m.Username == model.LoginAccount);
                if (user != null)
                {
                    if (user.Password.Trim() == model.LoginPassword.Trim())
                    {
                        tokenResult.Result = 1;
                        User loginUser = UserCache.GetUserById(user.Id);
                        if (loginUser == null)
                        {
                            UserCache.SetUser(user);
                            loginUser = UserCache.GetUserById(user.Id);
                        }
                        tokenResult.Data = loginUser;
                        tokenResult.AuthUser = loginUser;
                        tokenResult.AuthToken = CreateAuthUserAccessToken(loginUser);
                        tokenResult.ExpiredTime = DateTime.Now.AddMinutes(10);
                        tokenResult.InvalidTime = DateTime.Now.AddDays(30);
                    }
                    else
                    {
                        tokenResult.Result = -1;
                        //0:失败,-1:密码错误,-2:用户不存在,>0:用户ID
                        tokenResult.Message = "密码错误";


                    }
                }
                else
                {
                    tokenResult.Result = -2;
                    //0:失败,-1:密码错误,-2:用户不存在,>0:用户ID
                    tokenResult.Message = "用户不存在";


                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                tokenResult.Result = -500;
                tokenResult.Message = "授权出错，请刷新重试";
            }

            return tokenResult;
        }

        /// <summary>
        /// 使用授权令牌获取对应的用户数据
        /// </summary>
        /// <param name="accessToken">授权令牌</param>
        /// <returns>用户数据</returns>
        public User GetUserByAccessToken(string accessToken)
        {

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("invalid access_token!");
            }
            else
            {
                try
                {
                    string decryptStr = SecurityHelper.DESCryptoDecode(accessToken);
                    string[] info = decryptStr.Split('|');
                    if (info != null && info.Length >= 5)
                    {
                        int userId = ConvertHelper.ToInt32(info[0], 0);
                        string password = info[1];
                        long refreshTimestamp = ConvertHelper.ToInt64(info[2], 0);
                        long createdTimestamp = ConvertHelper.ToInt64(info[3], 0);
                        string rand = info[4];

                        long nowTimestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);
                        if (nowTimestamp - createdTimestamp > 2592000) //超过30天，需要重新授权
                        {
                            throw new Exception("access_token expired!");
                        }
                        else
                        {
                            if (nowTimestamp - refreshTimestamp > 600) //过期，需要刷新
                            {
                                throw new Exception("access_token needs to be refreshed!");
                            }

                            User user = UserCache.GetUserById(userId);

                            if (user != null && string.Compare(user.Password, password, true) == 0)
                            {
                                return user;
                            }
                            else
                            {
                                return null;
                                //throw new Exception("password has been changed!");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("invalid access_token!");
                    }
                }
                catch
                {
                    throw new Exception("invalid access_token!");
                }
            }

        }

        /// <summary>
        /// 刷新令牌，根据原令牌刷新获取新的令牌
        /// (需要Access_Token)
        /// </summary>
        /// <returns>授权令牌。结果错误码(0:出错,-1:错误令牌,-3:令牌过期,-4:密码已变更)</returns>
        public UserAccessToken RefreshUserAccessToken(string accessToken)
        {

            UserAccessToken tokenResult = new UserAccessToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                tokenResult.Result = 0;
                tokenResult.Message = "invalid access_token!";
            }
            else
            {
                try
                {
                    string decryptStr = SecurityHelper.DESCryptoDecode(accessToken);
                    string[] info = decryptStr.Split('|');
                    if (info != null && info.Length >= 5)
                    {
                        int userId = ConvertHelper.ToInt32(info[0], 0);
                        string password = info[1];
                        long refreshTimestamp = ConvertHelper.ToInt64(info[2], 0);
                        long createdTimestamp = ConvertHelper.ToInt64(info[3], 0);
                        string rand = info[4];

                        long nowTimestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);
                        if (nowTimestamp - createdTimestamp > 2592000) //超过30天，需要重新授权
                        {
                            tokenResult.Result = -3;
                            tokenResult.Message = "access_token expired!";
                        }
                        else
                        {
                            string newToken = BuildAuthUserAccessToken(userId, password, nowTimestamp, createdTimestamp, rand);
                            User user = GetSingle(m => m.Id == userId);
                            if (user != null && string.Compare(user.Password, password, true) == 0)
                            {
                                UserCache.SetUser(user);
                                tokenResult.Result = 1;
                                tokenResult.AuthUser = user;
                                tokenResult.AuthToken = newToken;
                                tokenResult.ExpiredTime = DateTime.Now.AddMinutes(10);
                                tokenResult.InvalidTime = DateTimeHelper.ConvertToDateTime(createdTimestamp + 2592000);
                            }
                            else
                            {
                                tokenResult.Result = -4;
                                tokenResult.Message = "password has beed changed!";
                            }
                        }
                    }
                    else
                    {
                        tokenResult.Result = -1; //错误令牌，需要重新授权
                        tokenResult.Message = "invalid access_token!";
                    }
                }
                catch
                {
                    tokenResult.Result = 0;
                    tokenResult.Message = "invalid access_token!";
                }
            }
            return tokenResult;
        }

        /// <summary>
        /// 应用层从Cookie的AccessToken获取用户详情
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="status">获取状态(-1:错误令牌,-2:过期(需刷新),-3:需要重新授权,-4:密码已更改,
        /// 0:无效令牌,-5:系统服务已被停用,-6:系统服务已到期)</param>
        /// <param name="authAccessToken">返回令牌</param>
        /// <returns>登录用户</returns>
        public User GetUserFromAccessTokenCookie(WebSession session, string authAccessToken, out int status)
        {
            status = 0;
            User authUser = null;

            if (!string.IsNullOrEmpty(authAccessToken))
            {
                #region 根据令牌获取授权用户
                User tokenUser;
                int authUserId = GetAuthUserFromAccessToken(authAccessToken, out tokenUser);

                if (authUserId <= 0)
                {
                    session.Remove(Constants.SESSION_KEY_CURRENT_USER);
                    switch (authUserId)
                    {
                        case -2:
                            #region 刷新令牌
                            UserAccessToken newAccessToken = RefreshUserAccessToken(authAccessToken);
                            if (newAccessToken.Result > 0)
                            {
                                authAccessToken = newAccessToken.AuthToken;
                                authUserId = GetAuthUserFromAccessToken(newAccessToken.AuthToken, out tokenUser);

                                WebHelper.SetCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN, newAccessToken.AuthToken, Constants.COOKIE_DOMAIN, newAccessToken.InvalidTime);
                            }
                            else
                            {
                                authUserId = newAccessToken.Result.Value;
                            }
                            #endregion
                            break;
                    }
                }

                status = authUserId;

                if (authUserId > 0 && tokenUser != null)
                {
                    #region 获取用户数据并设置授权用户Session
                    authUser = session.Get<User>(Constants.SESSION_KEY_CURRENT_USER);

                    if (authUser == null)
                    {
                        authUser = GetUserByAccessToken(authAccessToken);
                    }

                    if (authUser != null)
                    {
                        if (string.Compare(authUser.Password, tokenUser.Password, true) != 0)
                        {
                            authUser = null;
                            session.Remove(Constants.SESSION_KEY_CURRENT_USER);
                            WebHelper.RemoveCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN, Constants.COOKIE_DOMAIN);
                            WebHelper.RemoveCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN);
                        }
                        else
                        {
                            session.Set<User>(Constants.SESSION_KEY_CURRENT_USER, authUser);
                        }
                    }
                    #endregion
                }
                #endregion
            }

            return authUser;
        }
        #endregion

        #region 新版-使用JWT令牌

        /// <summary>
        /// 获取登录用户的JsonWebToken令牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserAccessToken GetJsonWebToken(LoginViewModel model)
        {
            UserAccessToken tokenResult = new UserAccessToken();

            try
            {
                #region 数据检查

                if (string.IsNullOrEmpty(model.LoginAccount))
                {
                    tokenResult.Result = -1;
                    tokenResult.Message = "账号不能为空";

                    return tokenResult;
                }
                if (string.IsNullOrEmpty(model.LoginPassword))
                {
                    tokenResult.Result = -1;
                    tokenResult.Message = "密码不能为空";

                    return tokenResult;
                }

                #endregion
                User user = repository.GetSingle(m => m.Username == model.LoginAccount );
                if (user != null)
                {
                    if (user.Password.Trim() == model.LoginPassword.Trim())
                    {
                        
                        tokenResult.Result = 1;

                        IDateTimeProvider provider = new UtcDateTimeProvider();
                        var now = provider.GetNow();

                        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
                        var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds).ToLong();
                        JwtAuthUser payload = new JwtAuthUser
                        {
                            Roles = new List<string> { user.UserType.ToString() },
                            UserId = user.Id,
                            UserName = user.Username,
                            Exp = secondsSinceEpoch + 7200,
                        };
                        IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                        IJsonSerializer serializer = new JsonNetSerializer();
                        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                        IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                        byte[] secret = Encoding.Default.GetBytes(ConfigurationManager.AppSettings["SecureKey"]);
                        var token = encoder.Encode(payload, secret);
                        UserCache.SetUser(user, 7200);
                        tokenResult.AuthToken = token;
                    }
                    else
                    {
                        tokenResult.Result = -1;
                        //0:失败,-1:密码错误,-2:用户不存在,>0:用户ID
                        tokenResult.Message = "密码错误";


                    }
                }
                else
                {
                    tokenResult.Result = -2;
                    //0:失败,-1:密码错误,-2:用户不存在,>0:用户ID
                    tokenResult.Message = "用户不存在";


                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.InnerException);

                tokenResult.Result = -500;
                tokenResult.Message = "授权出错，请刷新重试";
            }

            return tokenResult;
        }

        /// <summary>
        /// 从JsonWebToken获取用会话户详情
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="status">获取状态(0:无效令牌,-1:错误令牌,-2:过期(需刷新),-3:需要重新授权,-4:密码已更改,
        /// -5:系统服务已被停用,-6:系统服务已到期)</param>
        /// <param name="authAccessToken">令牌</param>
        /// <param name="newAccessToken">刷新后的令牌</param>
        /// <returns>登录用户</returns>
        public User GetSessionUserFromJsonWebToken(string authAccessToken, out int status, out UserAccessToken newAccessToken)
        {
            status = 0;
            newAccessToken = null;
            User authUser = null;
            if (!string.IsNullOrEmpty(authAccessToken))
            {
                #region 根据令牌获取授权用户
                if (!string.IsNullOrEmpty(authAccessToken))
                {
                    try
                    {
                        string secret = ConfigurationManager.AppSettings["SecureKey"].ToString();
                        IJsonSerializer serializer = new JsonNetSerializer();
                        IDateTimeProvider provider = new UtcDateTimeProvider();
                        IJwtValidator validator = new JwtValidator(serializer, provider);
                        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                        IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                        var json = decoder.Decode(authAccessToken, secret, verify: true);//token为之前生成的字符串
                        JwtAuthUser authInfo = JsonConvert.DeserializeObject<JwtAuthUser>(json);
                        if (authInfo != null)
                        {
                            User user = UserCache.GetUserById(authInfo.UserId);
                            long starTimestamp = authInfo.Exp - 600;//过期前十分钟
                            long nowTimestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);
                            if (nowTimestamp > starTimestamp && nowTimestamp < authInfo.Exp)//过期前十分钟刷新token
                            {
                                if (user != null)
                                {
                                    var now = provider.GetNow();
                                    var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
                                    var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds).ToLong();
                                    JwtAuthUser payload = new JwtAuthUser
                                    {
                                        Roles = new List<string> { user.UserType.ToString() },
                                        UserId = user.Id,
                                        UserName = user.Username,
                                        Exp = secondsSinceEpoch + 7200,
                                    };
                                    IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                                    IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                                    var token = encoder.Encode(payload, secret);
                                    UserCache.SetUser(user, 7200);
                                    newAccessToken = new UserAccessToken();
                                    newAccessToken.AuthToken = token;
                                }
                                else
                                {
                                    status = -1; //错误令牌，需要重新授权
                                }
                            }
                            else if (nowTimestamp > authInfo.Exp)
                            {
                                status = -2;//令牌过期 
                            }
                            else
                            {
                                authUser = user;
                            }
                        }
                        else
                        {
                            status = -1; //错误令牌，需要重新授权
                        }
                    }
                    catch (Exception ex)
                    {
                        status = -1; //错误令牌，需要重新授权
                        LogHelper.WriteLog("根据令牌获取授权用户异常:" + ex.ToString());
                    }
                }
                #endregion
            }

            return authUser;
        }

        #endregion
    }
    public class JwtAuthUser
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 角色列表，可以用于记录该用户的角色
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// 令牌过期时间
        /// </summary>
        public long Exp { get; set; }
    }
}
