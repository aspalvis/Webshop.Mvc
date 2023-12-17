using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Display order")]
        [Range(1, int.MaxValue, ErrorMessage = "Min value is 1")]
        public int DisplayOrder { get; set; }
    }
}
