using Foodie.Helpers;
using Foodie.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foodie.Controllers
{
    public class ReviewController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["FoodieDatabase"].ConnectionString;
        // GET: Review
        public ActionResult Index(List<Foodie.Models.Review> modelList)
        {
            return View(modelList);
        }

        //GET: create Review
        public ActionResult Create(string restaurantId)
        {
            Restaurant restaurant = Querries.getRestaurant(restaurantId);
            ViewBag.restaurantId = restaurantId;
            CreateReviewModel cModel = new CreateReviewModel();
            cModel.RestaurantId = restaurantId;
            cModel.RestaurantName = restaurant.Name;
            return View(cModel);
        }
        //POST: create Review
        [HttpPost]
        public ActionResult Create(Foodie.Models.CreateReviewModel model)
        {
            Review review = CreateReview(model);
            return RedirectToAction("Details", "Review", new {reviewId = review.ReviewId.ToString()});
        }

        //GET: review details page
        public ActionResult Details(string reviewId)
        {
            //later we can consolidate this into 1 stored procedure instead of 2
            Review review = Querries.getReview(reviewId);
            review.comments = Querries.getReviewComments(reviewId);
            return View(review);
        }

        [HttpPost]
        public JsonResult rateHelpfulness(string reviewId, int rating, string authorId)
        {
            HelpfullnessViewModel helpModel = new HelpfullnessViewModel();
            Guid personId = (Guid)Session["pId"];
            helpModel.HelpfulId = Guid.NewGuid().ToString();
            helpModel.RatingUserId = personId.ToString();
            helpModel.ReviewId = reviewId;
            helpModel.Rating = rating;
            helpModel.AuthorId = authorId;
            double newAverage = Querries.rateReviewHelpfullness(helpModel);
            return Json(newAverage, JsonRequestBehavior.AllowGet);
        }
            
        //}

        /// <summary>
        /// Create a Review
        /// </summary>
        /// <param name="?"></param>
        /// <returns> the created Review model</returns>
        private Review CreateReview(CreateReviewModel crModel)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                using (NpgsqlCommand command = conn.CreateCommand())
                {
                    conn.Open();
                    Review review = new Review();
                    int reviewId = 0;
                    //command.CommandText = string.Format(CultureInfo.InvariantCulture, "select max(UserId) from \"{0}\"", "Reviews");
                    //command.Prepare();
                    //reviewId = Convert.ToInt32(command.ExecuteScalar());
                    //incrementing pid
                    //reviewId = reviewId + 1;



                    //grab the user who's currently logged in
                    Guid personId = (Guid)Session["pId"];
                    string userName = (string)Session["Username"];
                    review.ReviewId = Guid.NewGuid();
                    review.UserId = personId;
                    review.UserName = userName;
                    review.ReviewText = crModel.ReviewText;
                    review.RestaurantName = crModel.RestaurantName;
                    review.RestaurantId = new Guid(crModel.RestaurantId);
                    review.DatePosted = DateTime.Now;
                    review.Rating = crModel.Rating;
                    using (NpgsqlCommand comm = conn.CreateCommand())
                    {
                        comm.CommandText = string.Format(CultureInfo.InvariantCulture,
                            "INSERT INTO \"Reviews\" (\"ReviewId\", \"ReviewText\", \"AverageReviewRating\", \"DatePosted\", \"UserId\", \"RestaurantId\", \"Rating\") Values (@ReviewId, @ReviewText, @AverageReviewRating, @DatePosted, @UserId, @RestaurantId, @Rating)");
                        comm.Parameters.Add("@ReviewId", NpgsqlDbType.Char, 36).Value = review.ReviewId;
                        comm.Parameters.Add("@ReviewText", NpgsqlDbType.Text).Value = review.ReviewText;
                        comm.Parameters.Add("@AverageReviewRating", NpgsqlDbType.Real).Value = 0.0;
                        comm.Parameters.Add("@DatePosted", NpgsqlDbType.Date).Value = review.DatePosted;
                        comm.Parameters.Add("@UserId", NpgsqlDbType.Char, 36).Value = review.UserId;
                        comm.Parameters.Add("@RestaurantId", NpgsqlDbType.Char, 36).Value = review.RestaurantId;
                        comm.Parameters.Add("@Rating", NpgsqlDbType.Integer).Value = review.Rating;
                        try
                        {
                            comm.Prepare();
                            if (comm.ExecuteNonQuery() > 0)
                            {
                                //Console.WriteLine("Success");
                            }
                            else
                            {
                                //Console.WriteLine("Failure");
                            }

                        }
                        catch (NpgsqlException e)
                        {
                            Trace.WriteLine(e.ToString());
                        }
                        finally
                        {
                            if (conn != null)
                            {
                                conn.Close();
                            }
                        }

                    }

                    return review;
                }
            }
        }
    }
}