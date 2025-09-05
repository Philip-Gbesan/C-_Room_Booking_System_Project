using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Project
{
    public static class BookingExtensions
    {
        // Extension method for calculating price with discount & timezone adjustment
        public static decimal CalculateTotalPriceWithDiscount(this Booking booking, Room room, TimeZoneInfo userTimeZone)
        {
            // Convert dates to user timezone
            DateTime start = TimeZoneInfo.ConvertTimeFromUtc(booking.StartDate.ToUniversalTime(), userTimeZone);
            DateTime end = TimeZoneInfo.ConvertTimeFromUtc(booking.EndDate.ToUniversalTime(), userTimeZone);

            int totalDays = (end - start).Days;
            if (totalDays <= 0) totalDays = 1; // At least 1 day

            decimal totalPrice = room.PricePerNight * totalDays;

            // Apply discount if more than 3 days
            if (totalDays > 3)
                totalPrice *= 0.9m;

            booking.NumberOfDays = totalDays;
            booking.Price = totalPrice;

            return totalPrice;
        }
    }

    public class BookingService
    {
        private readonly List<Room> rooms = new();
        private readonly List<Booking> bookings = new();

        public Room? GetRoomById(int roomId)
        {
            return rooms.FirstOrDefault(r => r.Id == roomId);
        }

        public void AddRoom(Room room)
        {
            rooms.Add(room);
        }

        public void DeleteRoomInfo(int roomId)
        {
            var room = GetRoomById(roomId);
            if (room != null)
                rooms.Remove(room);
        }

        public Booking BookRoom(int customerId, int roomId, DateTime startDate, DateTime endDate, TimeZoneInfo tz)
        {
            var room = GetRoomById(roomId);
            if (room == null) throw new Exception("Room not found");

            var booking = new Booking
            {
                Id = bookings.Count + 1,
                CustomerId = customerId,
                RoomId = roomId,
                StartDate = startDate,
                EndDate = endDate
            };

            // Use extension method
            booking.CalculateTotalPriceWithDiscount(room, tz);

            bookings.Add(booking);
            return booking;
        }

        public IEnumerable<Booking> GetAllBookings() => bookings;
    }
}
