using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSharingApp.Universal.Models
{
    public class PetDeposition
    {
        public ReturnUser User { get; set; }
        public DateTime DepositDate { get; set; }
        public DateTime WithDate { get; set; }
        public string Note { get; set; }
        public PetCareType PetCareType { get; set; }
    }
}
