using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSharingApp.Universal.Models
{
    class AddToCart
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int AccessoryId { get; set; }
        public int Quantity { get; set; }
    }
}
