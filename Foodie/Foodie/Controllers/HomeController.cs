using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foodie.Controllers
{
    public class HomeController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["FoodieDatabase"].ConnectionString;
        private const string restaurantTable = "Restaurants";

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
        public ActionResult Search(string query)
        {
            List<Foodie.Models.Restaurant> model = SearchResults(query);
            if (model.Count == 0 || model == null)
            {
                return View("NoResults");
            }
            return View(model);
        }

        private List<Foodie.Models.Restaurant> SearchResults(string query){
            List<Foodie.Models.Restaurant> model = new List<Foodie.Models.Restaurant>();

            if (string.IsNullOrEmpty(query)) { return null; }

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    //Stored Procedure that executes a search on a query
                    //This returns a cursor to the data set need to do this in a transaction
                    NpgsqlTransaction transaction = conn.BeginTransaction();
                    NpgsqlCommand command = conn.CreateCommand();
                    //Yes this is vulnerable to sql injection will sanitize this later.....
                    command.CommandText = string.Format(CultureInfo.InvariantCulture, "search(" + "'" + query.ToLower() + "'" + ")");
                    command.Transaction = transaction;
                    command.CommandType = CommandType.StoredProcedure;

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Foodie.Models.Restaurant temp = new Foodie.Models.Restaurant();
                            temp.RestaurantId = Guid.Parse(reader.GetValue(0).ToString());
                            temp.Name = reader.GetString(1);
                            temp.FoodType = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            temp.City = reader.GetString(3);
                            temp.State = reader.GetString(4);
                            temp.Country = reader.GetString(5);
                            //temp.Location = reader.IsDBNull(6) ? string.Empty : reader.G(6);
                            temp.Address = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                            model.Add(temp);
                        }
                        reader.Close();
                        transaction.Commit();
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
            return model;
        }
    }
}
