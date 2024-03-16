using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AnotherBooks
    {
        [Required(ErrorMessage = "{0} is missing")]
        [Display(Name = "BookID1")]
        public int? BookID1 { get; set; }

        [Required(ErrorMessage = "{0} is missing")]
        [Display(Name = "Book Name1")]
        public string BookName1 { get; set; }
        [Required(ErrorMessage = "{0} is missing")]
        [Display(Name = "Author Name1")]
        public string Author1 { get; set; }
    }
}
