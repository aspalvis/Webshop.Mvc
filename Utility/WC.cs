using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Utility
{
    public static class WC
    {
        public const string ImagePath = @"\images\product\";
        public const string SessionCart = "ShoppingCartSession";
        public const string SessionInquiryId = "InquirySession";

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string EmailAdmin = "admin@gmail.com";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "InProcess";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>
            (
                new List<string>
                {
                    StatusPending,
                    StatusApproved,
                    StatusInProcess,
                    StatusShipped,
                    StatusCancelled,
                    StatusRefunded
                }
            );

    }
}
