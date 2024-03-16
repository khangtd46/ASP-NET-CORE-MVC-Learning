using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [NotMapped]
		public string EncryptedId { get; set; }
		[Required(ErrorMessage = "Nhap Ten Ban oi")]
        public string Name { get; set; }
		[Required]
        [Display(Name = "Office Email")]
		public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }

        public string? PhotoPath { get; set; }
    }
}
