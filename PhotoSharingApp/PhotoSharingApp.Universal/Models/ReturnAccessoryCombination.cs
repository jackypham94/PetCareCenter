using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSharingApp.Universal.Models
{
    class ReturnAccessoryCombination
    {
        public ReturnAccessoryCategory Category { get; set; }
        public List<ReturnAccessory> ListOfAccessory { get; set; }
        public ReturnAccessoryCombination()
        {
            Category = new ReturnAccessoryCategory();
            ListOfAccessory = new List<ReturnAccessory>();
        }
    }
}
