using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{
    public class Books
    {
        [Required(ErrorMessage = "{0} is missing")]
        [Display(Name = "BookID")]
        public int? BookID { get; set; }

        [Required(ErrorMessage = "{0} is missing")]
        [Display(Name = "Book Name")]
      public string BookName { get; set; }
        [Required(ErrorMessage = "{0} is missing")]
        [Display(Name = "Author Name")]
        public string Author { get; set; }
    }
}
