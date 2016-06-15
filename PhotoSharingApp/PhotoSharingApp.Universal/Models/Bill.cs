using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSharingApp.Universal.Models
{
    public class Bill
    {
        public List<ReturnBuyingDetail> ReturnBuyingDetail { get; set; }
        public UserInfo UserInfo { get; set; }
        public Double Total { get; set; }
        public DateTime PlanDate { get; set; }
    }
}
