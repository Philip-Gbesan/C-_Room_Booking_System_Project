using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Project
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IRepository<Room> roomRepo = new InMemoryRepository<Room>();
            IRepository<Customer> customerRepo = new InMemoryRepository<Customer>();
            IRepository<Booking> bookingRepo = new InMemoryRepository<Booking>();

            var currencyService = new CurrencyService();
            await currencyService.UpdateRatesAsync();


            while (true)
            {
                Console.WriteLine("\n===== Booking System Menu =====");
                Console.WriteLine("1. View All Rooms");
                Console.WriteLine("2. Book Room");
                Console.WriteLine("3. Get Room By ID");
                Console.WriteLine("4. Cancel Booking");   
                Console.WriteLine("5. Reset All Data");
                Console.WriteLine("6. Supply Preset Data");
                Console.WriteLine("7. Exit");
                Console.Write("Select an option: ");
                string? choice = Console.ReadLine();


                switch (choice)
                {
                    case "1": // View All Rooms
                        Console.WriteLine("\nROOMS LIST:");

                        var allBookings = bookingRepo.GetAll().ToList();

                        foreach (var r in roomRepo.GetAll())
                        {
                            bool isBooked = allBookings.Any(b => b.RoomId == r.Id);
                            string status = isBooked ? "BOOKED" : "AVAILABLE";
                            Console.WriteLine($"Room #{r.Id} - {r.Type} - #{r.PricePerNight:N0} [{status}]");
                        }

                        Console.WriteLine("\nROOM AVAILABILITY BY TYPE:");
                        var grouped = roomRepo.GetAll()
                            .GroupBy(r => r.Type)
                            .Select(g => new
                            {
                                Type = g.Key,
                                Total = g.Count(),
                                Available = g.Count(r => allBookings.All(b => b.RoomId != r.Id))
                            });

                        foreach (var g in grouped)
                        {
                            Console.WriteLine($"{g.Type}: {g.Available}/{g.Total} available");
                        }
                        break;



                    case "2": // Book Room
                        Console.WriteLine("\nAvailable Room Types:");
                        // Group by type so user can pick category
                        var allTypes = roomRepo.GetAll()
                            .GroupBy(r => r.Type)
                            .Select((g, index) => new { Index = index + 1, Type = g.Key, Count = g.Count(r => bookingRepo.GetAll().All(b => b.RoomId != r.Id)) })
                            .ToList();

                        foreach (var t in allTypes)
                        {
                            Console.WriteLine($"{t.Index}. {t.Type} ({t.Count} available)");
                        }

                        Console.Write("Select Room Type (enter number): ");
                        if (!int.TryParse(Console.ReadLine(), out int typeChoice) || typeChoice < 1 || typeChoice > allTypes.Count)
                        {
                            Console.WriteLine("Invalid selection.");
                            break;
                        }

                        string selectedType = allTypes[typeChoice - 1].Type;

                        // Show available rooms of that type (not already booked)
                        var availableRooms = roomRepo.GetAll()
                            .Where(r => r.Type == selectedType && bookingRepo.GetAll().All(b => b.RoomId != r.Id))
                            .ToList();

                        if (!availableRooms.Any())
                        {
                            Console.WriteLine($"No available rooms of type {selectedType}.");
                            break;
                        }

                        Console.WriteLine($"\nAvailable {selectedType} Rooms:");
                        foreach (var r in availableRooms)
                        {
                            Console.WriteLine($"Room #{r.Id} - #{r.PricePerNight:N0}");
                        }

                        // Ask for specific room ID
                        Console.Write("Enter Room ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int roomId))
                        {
                            Console.WriteLine("Invalid room ID.");
                            break;
                        }

                        var roomToBook = availableRooms.FirstOrDefault(r => r.Id == roomId);
                        if (roomToBook == null)
                        {
                            Console.WriteLine("Room of that type with that ID not found or already booked.");
                            break;
                        }

                        // Customer info
                        Console.Write("Enter Full Name: ");
                        string name = Console.ReadLine()!;
                        var customer = new Customer { FullName = name, Email = "" };
                        customerRepo.Add(customer);

                        Console.Write("Enter Number of Days: ");
                        int days = int.Parse(Console.ReadLine()!);
                        DateTime startDate = DateTime.Now;
                        DateTime endDate = startDate.AddDays(days);

                        decimal totalPrice = roomToBook.PricePerNight * days;

                        if (days > 3)
                        {
                            totalPrice *= 0.9m;
                            Console.WriteLine("💰 10% discount applied for booking over 3 days.");
                        }

                        // Save booking
                        var booking = new Booking
                        {
                            RoomId = roomToBook.Id,
                            CustomerId = customer.Id,
                            StartDate = startDate,
                            EndDate = endDate
                        };
                        bookingRepo.Add(booking);


                        // Convert price based on region/timezone
                        var localTimeZone = TimeZoneInfo.Local;
                        string regionCode = MyHelperMethods.GetRegionCodeFromTimeZone(localTimeZone);
                        decimal convertedPrice = MyHelperMethods.ConvertPrice(totalPrice, regionCode, out string symbol);

                        // Confirmation
                        Console.WriteLine("\n===== BOOKING CONFIRMATION =====");
                        Console.WriteLine($"Name: {customer.FullName}");
                        Console.WriteLine($"Room ID: {roomToBook.Id}");
                        Console.WriteLine($"Room Type: {roomToBook.Type}");
                        Console.WriteLine($"Stay Duration: {days} day(s)");
                        Console.WriteLine($"Total Price (NGN): #{totalPrice:N0}");
                        Console.WriteLine($"Total Price ({symbol}): {convertedPrice:N2} [{regionCode}]");

                        break;



                    case "3": // GetRoomById
                        Console.Write("Enter Room ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int getId))
                        {
                            Console.WriteLine("Invalid input.");
                            break;
                        }

                        var foundRoom = roomRepo.GetById(getId);
                        if (foundRoom == null)
                        {
                            Console.WriteLine("Room not found.");
                            break;
                        }

                        Console.WriteLine($"\nRoom #{foundRoom.Id} - {foundRoom.Type} - #{foundRoom.PricePerNight:N0}");

                        // Check if this room has a booking
                        var bookings = bookingRepo.GetAll().FirstOrDefault(b => b.RoomId == foundRoom.Id);
                        if (bookings != null)
                        {
                            var customere = customerRepo.GetById(bookings.CustomerId);
                            int dayse = (int)(bookings.EndDate - bookings.StartDate).TotalDays;

                            decimal basePrice = foundRoom.PricePerNight * dayse;
                            decimal discount = dayse > 3 ? 0.10m : 0m;
                            decimal finalPrice = basePrice * (1 - discount);

                            Console.WriteLine("📌 BOOKING DETAILS:");
                            Console.WriteLine($"- Booked By: {customere?.FullName}");
                            Console.WriteLine($"- Stay Duration: {dayse} day(s)");
                            Console.WriteLine($"- Base Price: ₦{basePrice:N0}");
                            Console.WriteLine($"- Discount: {(discount * 100)}%");
                            Console.WriteLine($"- Price Paid: ₦{finalPrice:N0}");
                        }
                        else
                        {
                            Console.WriteLine("✅ This room is currently AVAILABLE.");
                        }
                        break;


                    case "4": // Cancel Booking
                        Console.Write("Enter Booking ID to cancel: ");
                        if (!int.TryParse(Console.ReadLine(), out int cancelId))
                        {
                            Console.WriteLine("Invalid booking ID.");
                            break;
                        }

                        var bookingToCancel = bookingRepo.GetById(cancelId);
                        if (bookingToCancel == null)
                        {
                            Console.WriteLine("Booking not found.");
                            break;
                        }

                        // Find the room that was booked
                        var bookedRoom = roomRepo.GetById(bookingToCancel.RoomId);
                        if (bookedRoom != null)
                        {
                            bookedRoom.IsBooked = false;  // ✅ Mark available again
                            roomRepo.Update(bookedRoom);
                        }

                        bookingRepo.Delete(cancelId); // ✅ Remove the booking record
                        Console.WriteLine($"Booking #{cancelId} has been cancelled if formerly booked. Room #{bookedRoom?.Id} is now available.");
                        break;

                    case "5":
                        MyHelperMethods.ResetAllData(roomRepo, customerRepo, bookingRepo);
                        break;

                    case "6":
                        MyHelperMethods.SupplyPresetData(roomRepo);
                        break;

                    case "7": // Exit
                        Console.WriteLine("Exiting... Press any key to close.");
                        Console.ReadKey();
                        return;


                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

    }


}
