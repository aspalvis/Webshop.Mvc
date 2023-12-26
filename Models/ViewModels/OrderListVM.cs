using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Models.ViewModels
{
    public class OrderListVM
    {
        public IEnumerable<OrderHeader> OrderHeaders { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        public string Status { get; set; }
    }
}
