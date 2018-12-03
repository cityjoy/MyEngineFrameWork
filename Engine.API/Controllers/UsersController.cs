using Engine.API.Filter;
using Engine.Application;
using Engine.Application.Model;
using Engine.Domain.Entity;
using Engine.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Engine.API.Controllers
{
    public class UsersController : BaseController
    {

        public UsersController(IUserServer userServer)
            : base(userServer)
        {
        }

        #region GET方法
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [Route("api/Users/GetUserList")]
        [HttpGet]
        [ApiAuthUserFilter]
        public IEnumerable<User> GetUserList()
        {
            return userServer.GetList().ToList();
        }
        
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Users/GetUserDetail")]
        [HttpGet]
        public User GetUserDetail(int id)
        {
            User user = userServer.GetSingle(m => m.Id == id);
            return user;

        }

         /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Users/GetJsonWebToken")]
        [HttpPost]
        public UserAccessToken GetJsonWebToken([FromBody] LoginViewModel model)
        {
            UserAccessToken token = userServer.GetJsonWebToken(model);
            return token;

        }
        #endregion
        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="model"></param>
        [Route("api/Users/SaveUser")]
        [HttpPost]
        public PageResult SaveUser([FromBody] User model)
        {
            PageResult result = new PageResult();
            bool r = userServer.Add(model);
            if (r)
            {
                result.Result = PageResultType.Success;
                result.Message = "ok";
            }
            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        [Route("api/Users/DeleteUser")]
        [HttpPost]
        public void DeleteUser(int id)
        {
        }
    }
}
