using System;
using System.Collections.Generic;

#nullable disable

namespace Smartshop_FrontEnd.Models
{
    public partial class Bill
    {
        public int Id { get; set; }
        public int? ShopperId { get; set; }
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }

        public virtual Shopper Shopper { get; set; }
    }
}
