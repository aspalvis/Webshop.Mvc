using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class OrderDetails
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderHeaderId { get; set; }

        [ForeignKey(nameof(OrderHeaderId))]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int Sqft { get; set; }

        public double PricePerSqFt { get; set; }
    }
}
