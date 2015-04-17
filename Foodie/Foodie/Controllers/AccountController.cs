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
            try{
                if (ModelState.IsValid && Login(model.UserName, model.Password))
                {
                    User user = GetUser(model.UserName, true);
                    Session["Username"] = user.Username;
                    Session["Email"] = user.Email;
                    Session["pId"] = user.pId;
                    Session["PasswordQuestion"] = user.PasswordQuestion;
                    Session["Approved"] = user.IsApproved;
                    Session["LockedOut"] = user.IsLockedOut;
                    Session["LastLoginDate"] = user.LastLoginDate;
                    Session["LastActivityDate"] = user.LastActivityDate;
                    Session["LastPasswordChangedDate"] = user.LastPasswordChangedDate;
                    Session["LastLockedOutDate"] = user.LastLockedOutDate;
                    Session["ProfileType"] = user.ProfileType;
                    return RedirectToAction("Index", "Home");
                }
            }
            // If we got this far, something failed, redisplay form
            catch(MembershipCreateUserException e){
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
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
                    return RedirectToAction("RegisterSuccess", "Account");
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
        /// Checks to see if the email used to create an account is unique
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private Boolean EmailUnique(String email)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                using (NpgsqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = string.Format(CultureInfo.InvariantCulture, "SELECT \"Email\" FROM \"{0}\" WHERE \"Email\" = @Email", userTable);

                    command.Parameters.Add("@Email", NpgsqlDbType.Varchar, 255).Value = email;
                    try
                    {
                        conn.Open();
                        command.Prepare();

                        using(NpgsqlDataReader reader = command.ExecuteReader()){
                            if(reader.HasRows){
                                return false;
                            }
                            return true;
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
                }
            }
            return false;
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
            //lets do some simple email checking
            if (string.IsNullOrEmpty(model.Email) || !(EmailUnique(model.Email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                throw new MembershipCreateUserException();
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
                        command.CommandText = string.Format(CultureInfo.InvariantCulture, "INSERT INTO \"{0}\" (\"pId\", \"Username\", \"Password\", \"Email\", \"PasswordQuestion\", \"PasswordAnswer\", \"CreationDate\", \"LastPasswordChangedDate\", \"LastActivityDate\", \"IsLockedOut\", \"LastLockedOutDate\", \"FailedPasswordAttemptCount\", \"FailedPasswordAttemptWindowStart\", \"FailedPasswordAnswerAttemptCount\", \"FailedPasswordAnswerAttemptWindowStart\", \"ProfileType\") Values (@pId, @Username, @Password, @Email, @PasswordQuestion, @PasswordAnswer, @CreationDate, @LastPasswordChangedDate, @LastActivityDate, @IsLockedOut, @LastLockedOutDate, @FailedPasswordAttemptCount, @FailedPasswordAttemptWindowStart, @FailedPasswordAnswerAttemptCount, @FailedPasswordAnswerAttemptWindowStart, @ProfileType)", userTable);
                        
                        command.Parameters.Add("@pId", NpgsqlDbType.Varchar, 36).Value = providerUserKey;
                        command.Parameters.Add("@Username", NpgsqlDbType.Varchar, 255).Value = model.UserName;
                        command.Parameters.Add("@Password", NpgsqlDbType.Varchar, 255).Value = EncryptPassword(model.Password);
                        command.Parameters.Add("@Email", NpgsqlDbType.Varchar, 255).Value = model.Email;
                        command.Parameters.Add("@PasswordQuestion", NpgsqlDbType.Varchar, 255).Value = model.SecurityQuestion;
                        command.Parameters.Add("@PasswordAnswer", NpgsqlDbType.Varchar, 255).Value = model.SecurityAnswer;
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

            //if we get here then something went wrong or there is an existing user name
            status = MembershipCreateStatus.DuplicateUserName;
            throw new MembershipCreateUserException();
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
                    command.CommandText = string.Format(CultureInfo.InvariantCulture, "SELECT \"pId\", \"Username\", \"Email\", \"PasswordQuestion\", \"IsLockedOut\", \"CreationDate\", \"LastLoginDate\", \"LastActivityDate\", \"LastPasswordChangedDate\", \"LastLockedOutDate\" FROM \"{0}\" WHERE \"Username\" = @Username", userTable);
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
                                user = new User();
                                user.pId = Guid.Parse(reader.GetValue(0).ToString());
                                user.Username = reader.GetString(1);
                                user.Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                user.PasswordQuestion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                user.IsLockedOut = reader.IsDBNull(5) ? false : reader.GetBoolean(4);
                                user.CreationDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5);
                                user.LastLoginDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6);
                                user.LastActivityDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7);
                                user.LastPasswordChangedDate = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8);
                                user.LastLockedOutDate = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9);
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
            String dbPassword = "";
            using(NpgsqlConnection connection = new NpgsqlConnection(connectionString)){
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(CultureInfo.InvariantCulture, "SELECT \"Password\" FROM \"{0}\" WHERE \"Username\" = @Username AND \"IsLockedOut\" = @IsLockedOut", userTable);
                    command.Parameters.Add("@Username", NpgsqlDbType.Varchar, 255).Value = username;
                    command.Parameters.Add("@IsLockedOut", NpgsqlDbType.Boolean).Value = false;

                    try
                    {
                        connection.Open();
                        command.Prepare();
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                dbPassword = reader.GetString(0);
                            }
                            //If no rows then username doesn't exist
                            else
                            {
                                status = MembershipCreateStatus.InvalidUserName;
                                throw new MembershipCreateUserException();
                            }
                        }
                    }
                    catch (NpgsqlException e)
                    {
                        //If it throws an error then it is most likely that the user name doesnt exist or is incorrect
                        Trace.WriteLine(e.ToString());
                    }
                    finally
                    {
                        if (connection != null)
                        {
                            connection.Close();
                        }
                    }
                }
                //Check against hash to see if its the same if it is means same password
                if (EncryptPassword(password).Equals(dbPassword))
                {
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = string.Format(CultureInfo.InvariantCulture, "UPDATE \"{0}\" SET \"LastLoginDate\" = @LastLoginDate WHERE \"Username\" = @Username", userTable);

                        command.Parameters.Add("@LastLoginDate", NpgsqlDbType.TimestampTZ).Value = DateTime.Now;
                        command.Parameters.Add("@Username", NpgsqlDbType.Varchar, 255).Value = username;

                        try
                        {
                            connection.Open();
                            command.Prepare();

                            command.ExecuteNonQuery();

                            return true;
                        }
                        catch (NpgsqlException e)
                        {
                            Trace.WriteLine(e.ToString());
                        }
                        finally
                        {
                            if (connection != null)
                            {
                                connection.Close();
                            }
                        }
                    }
                }
            }
            //Have to update the failure count for wromg password
            status = MembershipCreateStatus.InvalidPassword;
            throw new MembershipCreateUserException();
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

    public class ProfileController : Controller
    {
        public ActionResult Profile()
        {
            return View();
        }
    }
}
