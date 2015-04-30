using Foodie.Helpers;
using Foodie.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foodie.Controllers
{

    public class CommentController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["FoodieDatabase"].ConnectionString;
        // GET: Comment
        public ActionResult Create(string reviewId)
        {
            var newComment = new CommentViewModel();
            newComment.ReviewId = reviewId; // 
            newComment.UserId = (string)Session["pId"];
            newComment.UserName = (string)Session["userName"];

            return View(newComment);
        }

        [HttpPost]
        public ActionResult Create(CommentViewModel comment)
        {
            //create comment
            return View();
        }

        public ActionResult Details(string commentId)
        {

            //CommentViewModel comment = Querries.getComment(commentId);
            //return View(comment);
            return View();
        }

    }
}