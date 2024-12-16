using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Restore session from cookies if session is empty
        if (HttpContext.Session.GetString("UserId") == null)
        {
            var userId = Request.Cookies["UserId"];
            var userRole = Request.Cookies["UserRole"];
            var userName = Request.Cookies["UserName"];

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userRole) && !string.IsNullOrEmpty(userName))
            {
                HttpContext.Session.SetString("UserId", userId);
                HttpContext.Session.SetString("UserRole", userRole);
                HttpContext.Session.SetString("UserName", userName);
            }
        }

        // Populate ViewData for layout rendering
        ViewData["UserId"] = HttpContext.Session.GetString("UserId");
        ViewData["UserRole"] = HttpContext.Session.GetString("UserRole");
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        base.OnActionExecuting(context);
    }
}
