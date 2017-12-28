using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleASP.Models.DB;
using System.Net;
using System.Net.Mail;
using SampleASP.Models.ViewModels;
using System.Web.Security;

namespace SampleASP.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration( [Bind(Exclude = "IsVerified, Token, IsLoggedin")] User user)
        {
            bool Status = false;
            String Message = "";

            if (ModelState.IsValid)
            {
                if (IsUserNameExist(user.Username))
                {
                    ModelState.AddModelError("UsernameExists", "You can not use this username, its already taken!");
                }
                else
                { 
                    #region Generate Userid
                    user.Userid = Guid.NewGuid();
                    #endregion

                    #region Generate Token
                    user.Token = Guid.NewGuid();
                    #endregion
                     
                    #region Passowrd Hashing
                    user.Password = Crypto.Hash(user.Password);
                    user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                    #endregion

                    user.IsVerified = false;
                    user.IsLoggedin = false;

                    #region Saving to Database
                    using (SampleASPEntities SampleASPDatabase = new SampleASPEntities())
                    {
                        user.IsVerified = true;
                        user.IsLoggedin = false;
                        SampleASPDatabase.Users.Add(user);
                        SampleASPDatabase.SaveChanges();

                        #region Send Email Verification code
                        SendEmailVerificationCode(user.Email, user.Token);
                        Message = "Account created, verification code sent.";
                        #endregion

                        Status = true;
                    }
                    #endregion
              
                }
            }
            else
            {
                Message = "Invalid Message";
            }

            ViewBag.Status = Status;
            ViewBag.Message = Message;

            return View(user);
        }


        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            string Message = "";

            using (SampleASPEntities db = new SampleASPEntities())
            {
                var v = db.Users.Where(a => a.Token == new Guid(id)).FirstOrDefault();

                if (v != null)
                {
                    Status = true;
                    v.IsVerified = true;
                    db.SaveChanges();
                }
                else
                {
                    Message = "Invalid activation code!";
                }
            }

            ViewBag.Status = Status;
            ViewBag.Message = Message;

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin userLogin, string ReturnUrl)
        {
            var message = "";

            using (SampleASPEntities db = new SampleASPEntities())
            {
                var record = db.Users.Where(a => a.Username == userLogin.Username).FirstOrDefault();

                if (record != null && String.Equals(Crypto.Hash(userLogin.Password), record.Password))
                {
                    var timeout = userLogin.RememberMe ? 525600 : 20;
                    var ticket = new FormsAuthenticationTicket(userLogin.Username, userLogin.RememberMe, timeout);
                    var encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    message = "Invalid Username or Password!";
                }

            }

            ViewBag.Message = message;

            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        private bool IsUserNameExist(string Username)
        {
            using (SampleASPEntities SampleASPDatabase = new SampleASPEntities())
            {
                var v = SampleASPDatabase.Users.Where(a => a.Username == Username).FirstOrDefault();

                if (v != null)
                    return true;
            }

            return false;
        }

        [NonAction]
        private void SendEmailVerificationCode(string emailId, Guid token)
        {

            var verifyUrl = "/User/VerifyAccount/" + token;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("SampleASP1234@gmail.com", "SampleASP");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "RashedSourov1307045";

            var subject = "Account verification";

            var body = "<div>" +
                "To verify this email please go to the following link - " +
                "<a href = '" + link + "'>" + link + "</a>" +
                "</div>";


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}