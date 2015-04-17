using Foodie.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Foodie.Helpers
{
    public static class Querries
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["FoodieDatabase"].ConnectionString;

        /// <summary>
        /// querries the Restaurants table for the entry having the\n
        /// passed restaurantId and creates a Restaurant Model to represent it
        /// </summary>
        /// <param name="restaurantId">pId of desired restaurant entry</param>
        /// <returns>Model for requested restaurant entry</returns>
        public static Restaurant getRestaurant(string restaurantId)
        {
            Restaurant restaurant = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                using (NpgsqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM \"Restaurants\" WHERE \"RestaurantId\" = @RestaurantId";
                    command.Parameters.Add("@RestaurantId", NpgsqlDbType.Char, 36).Value = restaurantId;
                    try
                    {
                        conn.Open();
                        command.Prepare();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                restaurant = new Restaurant();
                                restaurant.RestaurantId = Guid.Parse(reader.GetValue(0).ToString());
                                restaurant.Name = reader.GetString(1);
                                restaurant.FoodType = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                restaurant.City = reader.GetString(3);
                                restaurant.State = reader.GetString(4);
                                restaurant.Country = reader.GetString(5);
                                //skipping location right now because idk how to handle the 'Point' postgres type
                                //  in this context
                                restaurant.Address = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                            }
                            else
                            {
                                Console.WriteLine("NO ROWS");
                            }
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
            return restaurant;
        }
    }
}