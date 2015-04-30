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
                                //Console.WriteLine("NO ROWS");
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

        /// <summary>
        /// querries the Reviews table for the entry having the\n
        /// passed reviewId and creates a Review Model to represent it
        /// </summary>
        /// <param name="reviewId">pId of desired review entry</param>
        /// <returns>Model for requested review entry</returns>
        public static Review getReview(string reviewId)
        {
            Review review = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                
                using (NpgsqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM \"Reviews\" WHERE \"ReviewId\" = @ReviewId";
                    command.Parameters.Add("@ReviewId", NpgsqlDbType.Char, 36).Value = reviewId;

                    
                    try
                    {
                        conn.Open();
                        command.Prepare();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                review = new Review();
                                review.ReviewId = Guid.Parse(reader.GetValue(0).ToString());
                                review.UserId = Guid.Parse(reader.GetValue(1).ToString());
                                review.RestaurantId = Guid.Parse(reader.GetValue(2).ToString());
                                review.ReviewText = reader.GetString(3);
                                review.Rating = reader.GetInt32(4);
                                review.DatePosted = reader.GetDateTime(5);
                                review.AverageReviewRating = reader.GetFloat(6);
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
            using (NpgsqlConnection conn2 = new NpgsqlConnection(connectionString))
            {
                try
                {
                    using (NpgsqlCommand command2 = conn2.CreateCommand())
                    {
                        conn2.Open();
                        command2.CommandText = "SELECT \"Username\" FROM \"Users\" WHERE \"pId\" = @UserId";
                        command2.Parameters.Add("@UserId", NpgsqlDbType.Char, 36).Value = review.UserId;
                        command2.Prepare();
                        using (NpgsqlDataReader reader2 = command2.ExecuteReader())
                        {
                            if (reader2.HasRows)
                            {
                                reader2.Read();
                                string name = reader2.GetString(0);
                                //validate
                                review.UserName = name;
                            }
                            else
                            {
                                //no rows
                            }
                        }
                    }
                    using (NpgsqlCommand command3 = conn2.CreateCommand())
                    {
                        command3.CommandText = "SELECT \"Name\" FROM \"Restaurants\" WHERE \"RestaurantId\" = @RestaurantId";
                        command3.Parameters.Add("@RestaurantId", NpgsqlDbType.Char, 36).Value = review.RestaurantId;
                        using (NpgsqlDataReader reader3 = command3.ExecuteReader())
                        {
                            if (reader3.HasRows)
                            {
                                reader3.Read();
                                string name = reader3.GetString(0);
                                //validate
                                review.RestaurantName = name;
                            }
                            else
                            {
                                //no rows
                            }
                        }
                    }
                }
                catch (NpgsqlException e)
                {
                    Trace.WriteLine(e.ToString());
                }
                finally
                {
                    if (conn2 != null)
                    {
                        conn2.Close();
                    }
                }
            }
            return review;
        }

        public static CommentViewModel getCommentInfo(string commentId) 
        {
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var trans = connection.BeginTransaction();

                using (var command = new Npgsql.NpgsqlCommand("getCommentInfo", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var idParam = command.CreateParameter();
                    idParam.ParameterName = "query";
                    idParam.DbType = System.Data.DbType.String;
                    idParam.Value = commentId;
                    command.Parameters.Add(idParam);

                    var da = new Npgsql.NpgsqlDataAdapter(command);
                    var ds = new System.Data.DataSet();
                    da.Fill(ds);
                    trans.Commit();
                    connection.Close();
                } 
            }
            return null;        
        }

        public static IEnumerable<CommentViewModel> getReviewComments(string commentId)
        {
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var trans = connection.BeginTransaction();

                using (var command = new Npgsql.NpgsqlCommand("getReviewComments", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var idParam = command.CreateParameter();
                    idParam.ParameterName = "query";
                    idParam.DbType = System.Data.DbType.String;
                    idParam.Value = commentId;
                    command.Parameters.Add(idParam);

                    var da = new Npgsql.NpgsqlDataAdapter(command);
                    var ds = new System.Data.DataSet();
                    da.Fill(ds);
                    trans.Commit();
                    connection.Close();
                }
            }

            return null;
        }
        
    }
}

