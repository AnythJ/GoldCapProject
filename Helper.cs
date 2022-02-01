using GoldCap.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap
{
    public class Helper
    {
        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }

        // Attributes
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.GetTypedHeaders().Referer == null ||
         filterContext.HttpContext.Request.GetTypedHeaders().Host.Host.ToString() != filterContext.HttpContext.Request.GetTypedHeaders().Referer.Host.ToString())
                {
                    filterContext.HttpContext.Response.Redirect("/");
                }
            }
        }

        public static int[] Last30DaysArray()
        {
            int manyThis = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int manyBefore = DateTime.DaysInMonth(DateTime.Now.AddDays(-30).Year, DateTime.Now.AddDays(-30).Month);
            if (DateTime.Now.AddDays(-30).Month != DateTime.Now.Month)
            {
                int daysInLastMonth = manyBefore - DateTime.Now.AddDays(-30).Day;
                int[] numberDayLastMonth = Enumerable.Range(DateTime.Now.AddDays(-30).Day, daysInLastMonth+1).ToArray();
                int daysInThisMoth = DateTime.Now.Day;
                int [] numberDayThisMonth = Enumerable.Range(1, daysInThisMoth).ToArray();

                int[] newArray = new int[numberDayLastMonth.Length + numberDayThisMonth.Length];
                Array.Copy(numberDayLastMonth, newArray, numberDayLastMonth.Length);
                Array.Copy(numberDayThisMonth, 0 , newArray, numberDayLastMonth.Length, numberDayThisMonth.Length);

                return newArray;
            }
            else
            {
                return Enumerable.Range(1, DateTime.Now.Day).ToArray();
            }
        }

       
    }
}
