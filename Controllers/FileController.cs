using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class FileController : Controller
    {
        //File/Download?IsLogged=true&id=200
        [Route("File/Download")]
        public IActionResult Index()
        {
            if (!Request.Query.ContainsKey("id"))
            {
                return BadRequest("Book ID not provided");
            }

            int id = Convert.ToInt32(Request.Query["id"]);
            if (id > 1000 || id < 1)
            {
                return NotFound("id mus be between 1 and 1000");
            }

            if (!Convert.ToBoolean(Request.Query["IsLogged"]))
                {
                return StatusCode(401);
            }
            return new VirtualFileResult("/Samples/TK1_TS_TuDuyKhang.pdf", "application/pdf");          
        }
    }
}
