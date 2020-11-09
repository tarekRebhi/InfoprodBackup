using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MVCWEB.Controllers
{
    public class JsonModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!IsJSONRequest(controllerContext))
            {
                return base.BindModel(controllerContext, bindingContext);
            }
            // get the JSON data that’s been posted
            var request = controllerContext.HttpContext.Request;
            var jsonStringData = new StreamReader(request.InputStream).ReadToEnd();
            return new JavaScriptSerializer()
                .Deserialize(jsonStringData, bindingContext.ModelMetadata.ModelType);
        }
        static bool IsJSONRequest(ControllerContext controllerContext)
        {
            var contentType = controllerContext.HttpContext.Request.ContentType;
            return contentType.Contains("application/json");
        }
        void AddModelBinders()
        {
            ModelBinders.Binders.DefaultBinder = new JsonModelBinder();
        }
    }
}