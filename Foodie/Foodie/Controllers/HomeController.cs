using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foodie.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "The page for Foodie's to congregate.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Meet the Foodie creator's";

                return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }
    }
}
