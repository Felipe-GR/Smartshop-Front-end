using System;
using System.Collections.Generic;

#nullable disable

namespace Smartshop_FrontEnd.Models
{
    public partial class Shopper
    {
        public Shopper()
        {
            Bills = new HashSet<Bill>();
        }

        public int Id { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
