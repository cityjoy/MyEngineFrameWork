using Engine.AdminWeb.Filter;
using Engine.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Engine.AdminWeb.Controllers
{
    [AuthorityFilter]
    public class HomeController : BaseController
    {
        public HomeController(IUserServer userServer)
            : base(userServer)
        {
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult Deom()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}