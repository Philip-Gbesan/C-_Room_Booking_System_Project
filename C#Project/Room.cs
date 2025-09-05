using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Project
{
    public class Room : IEntity
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public decimal PricePerNight { get; set; }
        public bool IsBooked { get; set; }   // ✅ Track booking status
    }

}
