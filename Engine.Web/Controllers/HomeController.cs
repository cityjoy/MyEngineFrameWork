using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Engine.Domain.Entity;
using Engine.Infrastructure.Utils;
using Engine.Web.Models;
using Engine.Web.Common;
using Engine.Application.Model;
using Engine.Application;
using Engine.Web.Filter;

namespace Engine.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserServer userServer):base(userServer)
        {
        }
        public ActionResult Index()
        {

            return View();
        }
        [AuthorityActionFilter]
        public ActionResult About()
        {

            return View();
        }
        
    }
}