using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class InquiryDetails
    {
        [Key]
        public int Id { get; set; }

        public int InquiryHeaderId { get; set; }

        [ForeignKey(nameof(InquiryHeaderId))]
        public InquiryHeader InquiryHeader { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

    }
}
