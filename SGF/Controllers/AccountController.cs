using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SGF.Models;


using System.Security.Cryptography;
using System.Text;
using SGF.Models.EF;

namespace SGF.Controllers
{
    public class AccountController : Controller
    {

        // GET: /Account/LogOn
        public ActionResult LogOn()
        {
            return View();
        }

        // POST: /Account/LogOn
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            //Valida que campos estão preenchidos
            if (model.Password == null || model.UserName == null)
            {
                ModelState.AddModelError("", "Username or Password is incorrect.");
                // If we got this far, something failed, redisplay form
                return View(model);
            }


            String Hashpass = EncryptSHA256(model.Password);
            //return RedirectToAction("Index", "Home");

            AccountModel user = ValidateUser(model.UserName, Hashpass);
            if (user != null)
            {
                Session["userOnline"] = user;

                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            } 
            else
                ModelState.AddModelError("", "Username or Password is incorrect.");

            // If we got this far, something failed, redisplay form
            return View(model);


            //if (ModelState.IsValid)
            //{
            //    if (Membership.ValidateUser(model.UserName, model.Password))
            //    {
            //        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
            //        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
            //            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            //        {
            //            return Redirect(returnUrl);
            //        }
            //        else
            //        {
            //            return RedirectToAction("Index", "Home");
            //        }
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", "The user name or password provided is incorrect.");
            //    }
            //}

            //// If we got this far, something failed, redisplay form
            //return View(model);
        }

        private string EncryptSHA256(string pass)
        {
            
            System.Security.Cryptography.SHA256CryptoServiceProvider x = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            //However in WinXp says "SHA256CryptoServiceProvider not supported for this platform"
            //System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(pass);
            data = x.ComputeHash(data);
            String sha256Hash = System.Text.Encoding.ASCII.GetString(data);
            return sha256Hash;
        }

        private AccountModel ValidateUser(string username, string password)
        {
            AccountModel user = null;
            using (SGFEntitiess sessionUser = new SGFEntitiess())
            {
                var result = from i in sessionUser.SGF_T_USER
                             where i.EMAIL == username && i.PASSWORD == password
                             select new { i.USER_ID, i.NAME, i.EMAIL };
                //result.ToList();
                if (result.SingleOrDefault() != null) 
                       user = new AccountModel(result.First().USER_ID, result.First().NAME, result.First().EMAIL);
            }
            return user;        
        }

        // GET: /Account/LogOff
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session["userOnline"] = null;
            return RedirectToAction("LogOn", "Account");
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            RegisterModel model;
            
            using (SGFEntitiess reg = new SGFEntitiess())
            {
                var c = from i in reg.SGF_R_COUNTRY
                                    orderby i.COUNTRY_NAME
                                    select i;
                var l = from i in reg.SGF_R_LANGUAGE
                            orderby i.LANGUAGE
                            select i;
                model = new RegisterModel
                {
                    Countries = c.ToList().AsEnumerable(),
                    Languages = l.ToList().AsEnumerable()
                };
            }

            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {

            // Attempt to register the user
            MembershipCreateStatus createStatus;
            CreateUser(model.Email,model.Password,model.Name,model.NationalityId,model.LanguageId,out createStatus);
            
            if ( createStatus == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
                return RedirectToAction("Register", "Account");
            }
            
            //if (ModelState.IsValid)
            //{
            //    // Attempt to register the user
            //    MembershipCreateStatus createStatus;
            //    Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

            //    if (createStatus == MembershipCreateStatus.Success)
            //    {
            //        FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
            //        return RedirectToAction("Index", "Home");
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", ErrorCodeToString(createStatus));
            //    }
            //}

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        private void CreateUser(string email, string password, string name, int nationality, 
                                int languagepref, out MembershipCreateStatus status)
        {
            using (SGFEntitiess sessionUser = new SGFEntitiess())
            {
                //Valida se existe já User registado
                var result = from i in sessionUser.SGF_T_USER
                             where i.EMAIL == email
                             select i;
                //Já existe o email em questão
                if (result.SingleOrDefault() != null)
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return;
                }

                string hashPassword = EncryptSHA256(password);
                SGF_T_USER s = SGF_T_USER.CreateSGF_T_USER(-1, name, hashPassword, email, DateTime.Now, nationality, languagepref);
                sessionUser.AddToSGF_T_USER(s);
                sessionUser.SaveChanges();

                AccountModel userOnline = new AccountModel(s.USER_ID, s.NAME, s.EMAIL);
                Session["userOnline"] = userOnline;
            }
            
            status = MembershipCreateStatus.Success;
            return;
        }

        // GET: /Account/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess"); 
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
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
        #endregion
    }
}
