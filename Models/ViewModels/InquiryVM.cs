using System.Collections.Generic;

namespace Models.ViewModels
{
    public class InquiryVM
    {
        public InquiryHeader InquiryHeader { get; set; }

        public List<InquiryDetails> InquiryDetails { get; set; }
    }
}
