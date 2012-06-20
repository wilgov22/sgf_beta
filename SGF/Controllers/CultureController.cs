using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace SGF.Controllers
{
    public class CultureController : Controller
    {

        public ActionResult SetCulture(string lang)
        {
            //Get Absolute URL Path of the Client Request who calls the SetCulture for redirect 
            string returnUrl = HttpContext.Request.UrlReferrer.AbsolutePath;
            Session["Language"] = new CultureInfo(lang);
            return Redirect(returnUrl);
        }

    }
}
