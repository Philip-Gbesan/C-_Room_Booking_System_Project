using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Project
{
    public class Booking : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Added properties
        public int NumberOfDays { get; set; }
        public decimal Price { get; set; } // Final price after discount

        public Booking() { }

        public Booking(int id, int customerId, int roomId, DateTime startDate, DateTime endDate, decimal price)
        {
            Id = id;
            CustomerId = customerId;
            RoomId = roomId;
            StartDate = startDate;
            EndDate = endDate;
            NumberOfDays = (EndDate - StartDate).Days;
            Price = price;
        }
    }
}
