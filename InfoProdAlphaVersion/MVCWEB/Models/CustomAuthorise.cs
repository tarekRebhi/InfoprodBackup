using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWEB.Models
{
    //this code is developped by imed. 
    public class CustomAuthorise : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If they are authorized, handle accordingly
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                // Otherwise redirect to your specific authorized area
                filterContext.Result = new RedirectResult("~/Authentification/Connect");
            }
        }
    }
}