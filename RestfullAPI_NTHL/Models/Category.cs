using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestfullAPI_NTHL.Models
{
    [Table("Category")] // 🔥 BẮT BUỘC
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(150)]
        public string CategoryName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Slug { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
