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
            return RedirectToAction("Details", review);
        }

        //GET: review details page
        public ActionResult Details(Foodie.Models.Review model)
        {
            return View(model);
        }

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
                    using (NpgsqlCommand comm = conn.CreateCommand())
                    {
                        comm.CommandText = string.Format(CultureInfo.InvariantCulture,
                            "INSERT INTO \"Reviews\" (\"ReviewId\", \"ReviewText\", \"AverageRating\", \"DatePosted\", \"UserId\", \"RestaurantId\") Values (@ReviewId, @ReviewText, @AverageRating, @DatePosted, @UserId, @RestaurantId)");
                        comm.Parameters.Add("@ReviewId", NpgsqlDbType.Char, 36).Value = review.ReviewId;
                        comm.Parameters.Add("@ReviewText", NpgsqlDbType.Text).Value = review.ReviewText;
                        comm.Parameters.Add("@AverageRating", NpgsqlDbType.Real).Value = 0.0;
                        comm.Parameters.Add("@DatePosted", NpgsqlDbType.Date).Value = review.DatePosted;
                        comm.Parameters.Add("@UserId", NpgsqlDbType.Char, 36).Value = review.UserId;
                        comm.Parameters.Add("@RestaurantId", NpgsqlDbType.Char, 36).Value = review.RestaurantId;
                        try
                        {
                            comm.Prepare();
                            if (comm.ExecuteNonQuery() > 0)
                            {
                                Console.WriteLine("Success");
                            }
                            else
                            {
                                Console.WriteLine("Failure");
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