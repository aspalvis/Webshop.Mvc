using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Short description")]
        public string ShortDescription { get; set; }

        [Range(0.1, double.MaxValue)]
        [DisplayName("Price")]
        public double Price { get; set; }

        public string Image { get; set; }

        [Display(Name = "Category type")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        [Display(Name = "Application type")]
        public int ApplicationTypeId { get; set; }

        [ForeignKey(nameof(ApplicationTypeId))]
        public virtual ApplicationType ApplicationType { get; set; }
    }
}
