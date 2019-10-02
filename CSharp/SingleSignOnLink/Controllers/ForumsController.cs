using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace SingleSignOnLink.Controllers
{
    [Route("forums")]
    public sealed class ForumsController : Controller
    {
        [HttpGet]
        [Route("{name}")]
        public IActionResult Forum(string name)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return Html("Sorry, you are not authorized to view this page.");
            }

            return Html(
                $"<p>Hi <strong>{User.Identity.Name}</strong>,", 
                $"Welcome to the <strong>{name}</strong> forum!",
                "<a href=/sso/signout>Click here to signout.</a>");
        }

        IActionResult Html(params string[] paragraphs)
        {
            var html = new StringBuilder();
            
            html.Append("<html>");
            html.Append("<body>");

            foreach (var paragraph in paragraphs)
            {
                html.Append("<p>");
                html.Append(paragraph);
                html.Append("</p>");
            }

            html.Append("</body>");
            html.Append("</html>");

            return Content(html.ToString(), "text/html");
        }
    }
}