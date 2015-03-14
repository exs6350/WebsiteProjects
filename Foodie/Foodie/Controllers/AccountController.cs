using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using System.Configuration;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Foodie.Models;
using Npgsql;
using System.Globalization;
using NpgsqlTypes;
using System.Diagnostics;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Foodie.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["FoodieDatabase"].ConnectionString;
        private const string userTable = "Users";
        private MachineKeySection machineKey = WebConfigurationManager.GetSection("system.web/machineKey") as MachineKeySection;
        MembershipCreateStatus status;
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    User newUser = CreateUser(model);
                    Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        /// <summary>
        /// Hashes a password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return password;
            }

            string encryptedPassword = password;

            HMACSHA1 hash = new HMACSHA1();
            hash.Key = HexToByte(machineKey.ValidationKey);
            encryptedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));

            return encryptedPassword;
        }

        /// <summary>
        /// Converts a hex string to bytes
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        /// <summary>
        /// Populates a user model based on the data given from the database
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private User GetUserFromReader(NpgsqlDataReader reader)
        {
            User user = new User();
            user.pId = Guid.Parse(reader.GetValue(0).ToString());
            user.Username = reader.GetString(1);
            user.Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            user.PasswordQuestion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            user.IsApproved = reader.IsDBNull(4) ? false : reader.GetBoolean(4);
            user.IsLockedOut = reader.IsDBNull(5) ? false : reader.GetBoolean(5);
            user.CreationDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6);
            user.LastLoginDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7);
            user.LastActivityDate = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8);
            user.LastPasswordChangedDate = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9);
            user.LastLockedOutDate = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10);
            return user;
        }

        private Boolean EmailUnique(RegisterModel model)
        {
            return true;
        }

        private Boolean UsernameUnique(RegisterModel model)
        {
            return true;
        }
        #endregion

        #region Account methods

        /// <summary>
        /// Allows a user to change their password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        private bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
            {
                return false;
            }

            int rowsaffected = 0;

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                using (NpgsqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = string.Format(CultureInfo.InvariantCulture, "UPDATE \"{0}\" SET \"Password\" = @Password, \"LastPasswordChangedDate\" = @LastPasswordChangedDate WHERE \"Username\" = @Username", userTable);

                    command.Parameters.Add("@Password", NpgsqlDbType.Varchar, 128).Value = EncryptPassword(newPassword);
                    command.Parameters.Add("@LastPasswordChangedDate", NpgsqlDbType.TimestampTZ).Value = DateTime.Now;
                    command.Parameters.Add("@Username", NpgsqlDbType.Varchar, 255).Value = username;

                    try
                    {
                        conn.Open();
                        command.Prepare();

                        rowsaffected = command.ExecuteNonQuery();
                    }
                    catch(NpgsqlException e){
                        Trace.WriteLine(e.ToString());
                    }
                    finally{
                        if(conn != null){
                            conn.Close();
                        }
                    }
                }
            }
            if(rowsaffected > 0){
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate's a user when they are trying to login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool ValidateUser(string username, string password)
        {
            return true;
        }

        /// <summary>
        /// Create a user account 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private User CreateUser(RegisterModel model)
        {
            Guid providerUserKey = model.pId;
            //lets do some simple username and email checking
            if (string.IsNullOrEmpty(model.Email) || !(EmailUnique(model)))
            {
                return null;
            }
            if (string.IsNullOrEmpty(model.UserName) || !(UsernameUnique(model)))
            {
                return null;
            }

            //if user doesn't exist already
            if (GetUser(model.UserName, false) == null)
            {
                DateTime createDate = DateTime.Now;

                if (model.pId == null || model.pId.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    providerUserKey = Guid.NewGuid();
                }

                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    using (NpgsqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = string.Format(CultureInfo.InvariantCulture, "INSERT INTO \"{0}\" (\"pId\", \"Username\", \"Password\", \"Email\", \"PasswordQuestion\", \"PasswordAnswer\", \"IsApproved\", \"CreationDate\", \"LastPasswordChangedDate\", \"LastActivityDate\", \"IsLockedOut\", \"LastLockedOutDate\", \"FailedPasswordAttemptCount\", \"FailedPasswordAttemptWindowStart\", \"FailedPasswordAnswerAttemptCount\", \"FailedPasswordAnswerAttemptWindowStart\", \"ProfileType\") Values (@pId, @Username, @Password, @Email, @PasswordQuestion, @PasswordAnswer, @IsApproved, @CreationDate, @LastPasswordChangedDate, @LastActivityDate, @IsLockedOut, @LastLockedOutDate, @FailedPasswordAttemptCount, @FailedPasswordAttemptWindowStart, @FailedPasswordAnswerAttemptCount, @FailedPasswordAnswerAttemptWindowStart, @ProfileType)", userTable);
                        
                        command.Parameters.Add("@pId", NpgsqlDbType.Varchar, 36).Value = providerUserKey;
                        command.Parameters.Add("@Username", NpgsqlDbType.Varchar, 255).Value = model.UserName;
                        command.Parameters.Add("@Password", NpgsqlDbType.Varchar, 255).Value = EncryptPassword(model.Password);
                        command.Parameters.Add("@Email", NpgsqlDbType.Varchar, 255).Value = model.Email;
                        command.Parameters.Add("@PasswordQuestion", NpgsqlDbType.Varchar, 255).Value = model.SecurityQuestion;
                        command.Parameters.Add("@PasswordAnswer", NpgsqlDbType.Varchar, 255).Value = model.SecurityAnswer;
                        command.Parameters.Add("@IsApproved", NpgsqlDbType.Boolean).Value = true;
                        command.Parameters.Add("@CreationDate", NpgsqlDbType.TimestampTZ).Value = createDate;
                        command.Parameters.Add("@LastPasswordChangedDate", NpgsqlDbType.TimestampTZ).Value = createDate;
                        command.Parameters.Add("@LastLockedOutDate", NpgsqlDbType.TimestampTZ).Value = createDate;
                        command.Parameters.Add("@LastActivityDate", NpgsqlDbType.TimestampTZ).Value = createDate;
                        command.Parameters.Add("@IsLockedOut", NpgsqlDbType.Boolean).Value = false;
                        command.Parameters.Add("@FailedPasswordAttemptCount", NpgsqlDbType.Integer).Value = 0;
                        command.Parameters.Add("@FailedPasswordAttemptWindowStart", NpgsqlDbType.TimestampTZ).Value = createDate;
                        command.Parameters.Add("@FailedPasswordAnswerAttemptCount", NpgsqlDbType.Integer).Value = 0;
                        command.Parameters.Add("@FailedPasswordAnswerAttemptWindowStart", NpgsqlDbType.TimestampTZ).Value = createDate;
                        command.Parameters.Add("@ProfileType", NpgsqlDbType.Integer).Value = model.ProfileType;

                        try{
                            conn.Open();
                            command.Prepare();

                            if(command.ExecuteNonQuery() > 0){
                                //success
                            }
                            else{
                                //something went wrong
                            }
                        }
                        catch(NpgsqlException e){
                            Trace.WriteLine(e.ToString());
                        }
                        finally{
                            if(conn != null){
                                conn.Close();
                            }
                        }
                        //now return the user 
                        return GetUser(model.UserName, false);
                    }
                }
            }
            //if we get here then something went wrong
            return null;
        }

        /// <summary>
        /// This is used to check if a user exists if it does then return it
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        private User GetUser(string username, bool userIsOnline)
        {
            User user = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                using (NpgsqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = string.Format(CultureInfo.InvariantCulture, "SELECT \"pId\", \"Username\", \"Email\", \"PasswordQuestion\", \"IsApproved\", \"IsLockedOut\", \"CreationDate\", \"LastLoginDate\", \"LastActivityDate\", \"LastPasswordChangedDate\", \"LastLockedOutDate\" FROM \"{0}\" WHERE \"Username\" = @Username", userTable);
                    command.Parameters.Add("@Username", NpgsqlDbType.Varchar, 255).Value = username;

                    try
                    {
                        conn.Open();
                        command.Prepare();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                user = GetUserFromReader(reader);
                                reader.Close();

                                if (userIsOnline)
                                {
                                    //update user online status
                                    using (NpgsqlCommand update = conn.CreateCommand())
                                    {
                                        update.CommandText = string.Format(CultureInfo.InvariantCulture, "UPDATE \"{0}\" SET \"LastActivityDate\" = @LastActivityDate WHERE \"pId\" = @pId", userTable);
                                        update.Parameters.Add("@LastActivityDate", NpgsqlDbType.TimestampTZ).Value = DateTime.Now;
                                        update.Parameters.Add("@pId", NpgsqlDbType.Char, 36).Value = user.pId;

                                        update.Prepare();

                                        update.ExecuteNonQuery();
                                    }
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
                        if (conn != null)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            return user;
        }

        /// <summary>
        /// This is used to login a user and persist the data to give access to different parts
        /// of the site that require an account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool Login(string username, string password)
        {
            return true;
        }

        /// <summary>
        /// This is used to delete the instance of a user 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="deleteAllData"></param>
        /// <returns></returns>
        private bool DeleteUser(string username, bool deleteAllData)
        {
            int rowsAffected = 0;

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                using (NpgsqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = string.Format(CultureInfo.InvariantCulture, "DELETE FROM \"{0}\" WHERE \"Username\" = @Username", userTable);
                    command.Parameters.Add("@Username", NpgsqlDbType.Varchar, 255).Value = username;

                    try
                    {
                        conn.Open();
                        command.Prepare();

                        rowsAffected = command.ExecuteNonQuery();

                        //Later on we mine the database to delete all other data for the user
                        if (deleteAllData)
                        {

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
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
