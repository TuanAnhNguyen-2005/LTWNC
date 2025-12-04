using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_ADMIN.Models
{
    public class LearningMaterial
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "URL")]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Display(Name = "Published Date")]
        public DateTime? PublishedDate { get; set; }
    }
}
