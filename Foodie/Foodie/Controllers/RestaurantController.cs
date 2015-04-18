using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Foodie.Models;
using Npgsql;
using System.Configuration;
using System.Globalization;
using NpgsqlTypes;
using System.Diagnostics;
using Foodie.Helpers;

namespace Foodie.Controllers
{
    public class RestaurantController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["FoodieDatabase"].ConnectionString;
        // GET: Restaurant
        public ActionResult Index()
        {
            return View();
        }

        //GET: create
        public ActionResult Create()
        {
            Restaurant restaurant = new Restaurant();
            return View(restaurant);
        }

        //POST: create
        [HttpPost]
        public ActionResult Create(Restaurant restaurant)
        {

            Guid pId = Guid.NewGuid();
            //validation
            //create new entry in table
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                using (NpgsqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = string.Format(CultureInfo.InvariantCulture, 
                        "INSERT INTO \"Restaurants\" (\"RestaurantId\", \"Name\", \"FoodType\", \"City\", \"State\", \"Country\", \"Location\", \"Address\") Values (@RestaurantId, @Name, @FoodType, @City, @State, @Country, @Location, @Address)");
                    command.Parameters.Add("@RestaurantId", NpgsqlDbType.Char, 36).Value = pId;
                    command.Parameters.Add("@Name", NpgsqlDbType.Varchar).Value = restaurant.Name;
                    command.Parameters.Add("@FoodType", NpgsqlDbType.Varchar).Value = restaurant.FoodType;
                    command.Parameters.Add("@City", NpgsqlDbType.Varchar).Value = restaurant.City;
                    command.Parameters.Add("@State", NpgsqlDbType.Varchar).Value = restaurant.State;
                    command.Parameters.Add("@Country", NpgsqlDbType.Varchar).Value = restaurant.Country;
                    command.Parameters.Add("@Location", NpgsqlDbType.Point).Value = null;
                    command.Parameters.Add("@Address", NpgsqlDbType.Varchar).Value = restaurant.Address;
                    try
                    {
                        conn.Open();
                        command.Prepare();
                        if (command.ExecuteNonQuery() > 0)
                        {
                            //Console.WriteLine("success");
                        }
                        else
                        {
                            //Console.WriteLine("failure");
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
            }
            //redirect to Details page for same restaurant object
            return RedirectToAction("Details", "Restaurant", new {restaurantId = pId });
        }

        public ActionResult Details(string restaurantId)
        {
            Restaurant restaurant = Querries.getRestaurant(restaurantId);
            return View(restaurant);
        }

        
    }
}