using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Engine.Domain.Entity;
using Engine.Infrastructure.Utils;
using Engine.Application;


namespace Engine.AdminWeb.Controllers
{
    public class AccountController : BaseController
    {

        public AccountController(IUserServer userServer)
            : base(userServer)
        {

        }
        public ActionResult AccountInfo()
        {
            return View();
        }
        public ActionResult SetPassword()
        {
            return View();
        }

        /// <summary>
        /// 保存账号
        /// </summary>
        /// <param name="updateAdmin"></param>
        /// <returns></returns>
        public JsonResult SaveAccount(User model)
        {
            OperateResult = new PageResult();
            bool isSaved = userServer.Update(m => m.Id == CurrentUser.Id, b =>
                      new User
                      {
                          RealName = model.RealName,
                          Phone=model.Phone,
                          EMail=model.EMail
                      });
            if (isSaved)
            {
                OperateResult.Result = PageResultType.Success;
                OperateResult.Message = "保存成功";
            }
            else
            {
                {
                    OperateResult.Result = PageResultType.Failed;
                    OperateResult.Message = "保存失败";
                }
            }

            return Json(OperateResult);
        }
        /// <summary>
        /// 保存账号密码
        /// </summary>
        /// <param name="oldpassword"></param>
        /// <param name="newpassword"></param>
        /// <param name="newpassword2"></param>
        /// <returns></returns>
       [ValidateAntiForgeryToken()]
        public JsonResult SavePassword(string oldpassword, string newpassword, string newpassword2)
        {
            OperateResult = new PageResult();
            string DESCryptPwd = SecurityHelper.DESCryptoEncode(oldpassword).ToLower();
            if (DESCryptPwd != CurrentUser.Password)
            {
                OperateResult.Result = PageResultType.Failed;
                OperateResult.Message = "原密码错误";
                return Json(OperateResult);
            }
            if (newpassword != newpassword2)
            {
                OperateResult.Result = PageResultType.Failed;
                OperateResult.Message = "两次密码不一致";
                return Json(OperateResult);

            }
            bool isSaved = userServer.Update(m => m.Id == CurrentUser.Id, b =>
                    new User
                    {
                        Password = SecurityHelper.DESCryptoEncode(newpassword).ToLower()
                    });
            if (isSaved)
            {
                OperateResult.Result = PageResultType.Success;
                OperateResult.Message = "保存成功";
            }

            return Json(OperateResult);
        }
    }
}