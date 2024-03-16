using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
      //  [Route("/Home")]
        public IActionResult Index()
        {
            return new ContentResult()
            {
                Content = "<h1>Welcome to Home Controller<h1>",
                ContentType = "text/html"
            };
        }

       // [Route("/Employee/John")]
        public ContentResult Employee()
        {
            return new ContentResult()
            {
                Content = "{\"name\":  \"john\"}",
                ContentType = "application/json"
            };
        }
        [Route("/Books/{bookid?}/{author?}/{BookName?}")]
        //[Route("/Books")]
        public IActionResult Books(Books book)
        {       
            if (!ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
               string errorMessages =  string.Join("\n", errors);
                return BadRequest(errorMessages);
            }
            return Content($"Book id is {book.BookID} \n {book.Author} \n {book.BookName}", "text/plain");
        }

    }
}
