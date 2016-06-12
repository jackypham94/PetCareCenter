using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSharingApp.Universal.Models
{
    class ReturnPet
    {
        public ReturnUser user { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public int? Gender { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
        public bool IsDepositing { get; set; }
        public ReturnPetCategory PetCategory { get; set; }
        public ReturnPet() { }
    }
}
