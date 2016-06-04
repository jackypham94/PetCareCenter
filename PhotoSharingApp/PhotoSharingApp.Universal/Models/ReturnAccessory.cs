using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSharingApp.Universal.Models
{
    class ReturnAccessory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockQuantity { get; set; }
        public double Price { get; set; }
        public byte[] Image { get; set; }
        public ReturnAccessory() { }
    }
}
