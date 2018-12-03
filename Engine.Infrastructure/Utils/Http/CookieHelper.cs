using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace  Engine.Infrastructure.Utils
{
    public class CookieHelper
    {
        #region 系统管理员
        /// <summary>
        /// 写后台管理员信息 cookie值
        /// </summary>
        /// <param name="userId">后台管理员Id</param>
        /// <param name="loginId">后台管理员名</param>
        /// <param name="name">姓名</param>
        /// <param name="emptype">后台管理员类别</param>
        public static void WriteManagersCookie(Guid userId, string loginId, string name, int roleId)
        {
            //首先删除
            DelManagersCookie();
            //然后添加
            HttpCookie MyCookie = new HttpCookie("Managers");//对象
            MyCookie.Values.Add("Id",StringEncryptHelper.DES_Encrypt(userId.ToString()));
            MyCookie.Values.Add("loginId", StringEncryptHelper.DES_Encrypt(HttpUtility.UrlEncode(loginId)));
            MyCookie.Values.Add("Name", StringEncryptHelper.DES_Encrypt(HttpUtility.UrlEncode(name)));
            MyCookie.Values.Add("ManagerRoleId", StringEncryptHelper.DES_Encrypt(roleId.ToString()));
            MyCookie.Expires = DateTime.Now.AddMinutes(30);
            MyCookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(MyCookie);
        }
        /// <summary>
        /// 删除后台管理员信息 cookie值
        /// </summary>
        public static void DelManagersCookie()
        {
            HttpCookie MyCookie = HttpContext.Current.Request.Cookies["Managers"];
            if (MyCookie != null)
            {
                MyCookie.Values.Add("Id", "");
                MyCookie.Values.Add("loginId", "");
                MyCookie.Values.Add("Name", "");
                MyCookie.Values.Add("ManagerRoleId", "");
                MyCookie.Expires = DateTime.Today.AddDays(-10);
                HttpContext.Current.Response.Cookies.Add(MyCookie);
            }
        }

        /// <summary>
        /// 注册后台管理员Cookie容器
        /// </summary>
        public static HttpCookie ManagerCookie
        {
            get
            {
                ChangeExpires("Managers");
                return System.Web.HttpContext.Current.Request.Cookies["Managers"];
            }
        }

        /// <summary>
        /// 后台管理员是否已登录
        /// </summary>
        /// <returns></returns>
        public static bool IsManagerLogin
        {
            get
            {
                if (ManagerCookie == null || ManagerId == Guid.Empty)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// 获取后台管理员Id
        /// </summary>
        public static Guid ManagerId
        {
            get
            {
                System.Web.HttpCookie objCookie = ManagerCookie;
                if (objCookie != null)
                {
                    Guid userId = Guid.Empty;
                    if (objCookie != null)
                    {
                        try { userId = new Guid(StringEncryptHelper.DES_Decrypt(objCookie.Values["Id"].ToString())); }
                        catch { }
                    }
                    return userId;
                }
                else { return Guid.Empty; }
            }
        }

        /// <summary>
        /// 获取后台管理员登录名
        /// </summary>
        public static string ManagerLoginId
        {
            get
            {
                System.Web.HttpCookie objCookie = ManagerCookie;
                if (objCookie != null)
                {
                    string loginId = "";
                    if (objCookie != null)
                    {
                        try { loginId = HttpUtility.UrlDecode(StringEncryptHelper.DES_Decrypt(objCookie.Values["loginId"].ToString())); }
                        catch { }
                    }
                    return loginId;
                }
                else { return ""; }
            }
        }
        /// <summary>
        /// 获取后台管理员姓名
        /// </summary>
        public static string ManagerName
        {
            get
            {
                System.Web.HttpCookie objCookie = ManagerCookie;
                if (objCookie != null)
                {
                    string name = "";
                    if (objCookie != null)
                    {
                        try { name = HttpUtility.UrlDecode(StringEncryptHelper.DES_Decrypt(objCookie.Values["Name"].ToString())); }
                        catch { }
                    }
                    return name;
                }
                else { return ""; }
            }
        }

        /// <summary>
        /// 后台管理员角色(ManagerRoleId)
        /// </summary>
        public static int ManagerRoleId
        {
            get
            {
                System.Web.HttpCookie objCookie = ManagerCookie;
                if (objCookie != null)
                {
                    int roleId = 0;
                    if (objCookie != null)
                    {
                        try { int.TryParse(StringEncryptHelper.DES_Decrypt(objCookie.Values["ManagerRoleId"].ToString()), out roleId); }
                        catch { }
                    }
                    return roleId;
                }
                else { return 0; }
            }
        }
        #endregion

        #region 用户
        /// <summary>
        /// 写后台用户信息 cookie值
        /// </summary>
        /// <param name="userId">后台用户Id</param>
        /// <param name="loginId">后台用户名</param>
        /// <param name="name">姓名</param>
        /// <param name="emptype">后台用户类别</param>
        public static void WriteUsersCookie(int userId, string loginId, string name, int roleId)
        {
            //首先删除
            DelUsersCookie();
            //然后添加
            HttpCookie MyCookie = new HttpCookie("Users");//对象
            MyCookie.Values.Add("Id", StringEncryptHelper.DES_Encrypt(userId.ToString()));
            MyCookie.Values.Add("loginId", StringEncryptHelper.DES_Encrypt(HttpUtility.UrlEncode(loginId)));
            MyCookie.Values.Add("Name", StringEncryptHelper.DES_Encrypt(HttpUtility.UrlEncode(name)));
            MyCookie.Values.Add("UserRoleId", StringEncryptHelper.DES_Encrypt(roleId.ToString()));
            MyCookie.Expires = DateTime.Now.AddMinutes(30);
            MyCookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(MyCookie);
        }
        /// <summary>
        /// 删除后台用户信息 cookie值
        /// </summary>
        public static void DelUsersCookie()
        {
            HttpCookie MyCookie = HttpContext.Current.Request.Cookies["Users"];
            if (MyCookie != null)
            {
                MyCookie.Values.Add("Id", "");
                MyCookie.Values.Add("loginId", "");
                MyCookie.Values.Add("Name", "");
                MyCookie.Values.Add("UserRoleId", "");
                MyCookie.Expires = DateTime.Today.AddDays(-10);
                HttpContext.Current.Response.Cookies.Add(MyCookie);
            }
        }

        /// <summary>
        /// 注册后台用户Cookie容器
        /// </summary>
        public static HttpCookie UserCookie
        {
            get
            {
                ChangeExpires("Users");
                return System.Web.HttpContext.Current.Request.Cookies["Users"];
            }
        }

        /// <summary>
        /// 后台用户是否已登录
        /// </summary>
        /// <returns></returns>
        public static bool IsUserLogin
        {
            get
            {
                if (UserCookie == null || UserId == 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// 获取后台用户Id
        /// </summary>
        public static int UserId
        {
            get
            {
                System.Web.HttpCookie objCookie = UserCookie;
                if (objCookie != null)
                {
                    int userId = 0;
                    if (objCookie != null)
                    {
                        try { userId = int.Parse(StringEncryptHelper.DES_Decrypt(objCookie.Values["Id"].ToString())); }
                        catch { }
                    }
                    return userId;
                }
                else { return 0; }
            }
        }

        /// <summary>
        /// 获取后台用户登录名
        /// </summary>
        public static string UserLoginId
        {
            get
            {
                System.Web.HttpCookie objCookie = UserCookie;
                if (objCookie != null)
                {
                    string loginId = "";
                    if (objCookie != null)
                    {
                        try { loginId = HttpUtility.UrlDecode(StringEncryptHelper.DES_Decrypt(objCookie.Values["loginId"].ToString())); }
                        catch { }
                    }
                    return loginId;
                }
                else { return ""; }
            }
        }
        /// <summary>
        /// 获取后台用户姓名
        /// </summary>
        public static string UserName
        {
            get
            {
                System.Web.HttpCookie objCookie = UserCookie;
                if (objCookie != null)
                {
                    string name = "";
                    if (objCookie != null)
                    {
                        try { name = HttpUtility.UrlDecode(StringEncryptHelper.DES_Decrypt(objCookie.Values["Name"].ToString())); }
                        catch { }
                    }
                    return name;
                }
                else { return ""; }
            }
        }

        /// <summary>
        /// 后台用户角色(UserRoleId)
        /// </summary>
        public static int UserRoleId
        {
            get
            {
                System.Web.HttpCookie objCookie = UserCookie;
                if (objCookie != null)
                {
                    int roleId = 0;
                    if (objCookie != null)
                    {
                        try { int.TryParse(StringEncryptHelper.DES_Decrypt(objCookie.Values["UserRoleId"].ToString()), out roleId); }
                        catch { }
                    }
                    return roleId;
                }
                else { return 0; }
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 调整cookie过期时间
        /// </summary>
        /// <param name="cookieName"></param>
        public static void ChangeExpires(string cookieName)
        {
            HttpCookie MyCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (MyCookie != null)
            {
                MyCookie.Expires = DateTime.Now.AddMinutes(30);
                HttpContext.Current.Response.AppendCookie(MyCookie);
            }
        }

        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">值</param>
        /// <returns></returns>
        public static T GetCookie<T>(string key)
        {
            HttpCookie hc = HttpContext.Current.Request.Cookies[key];
            if (hc != null)
            {
                string val = hc.Value;
                return Common.ConvertValue<T>(val);
            }
            return default(T);
        }

        /// <summary>
        /// 设置Cookie值
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">值</param>
        public static void SetCookie(string key, string value)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            HttpContext.Current.Response.SetCookie(cookie);
        }

        /// <summary>
        /// 删除Cookie值
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">值</param>
        public static void DelCookie(string key)
        {
            HttpCookie MyCookie = new HttpCookie(key);//对象
            MyCookie.Expires = System.DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
        }
        #endregion
    }
}