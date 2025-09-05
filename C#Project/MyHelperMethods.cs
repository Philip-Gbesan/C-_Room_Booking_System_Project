using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Project
{
    public static class MyHelperMethods
    {
        // Helper method to know the country region and time 
        public static string GetRegionCodeFromTimeZone(TimeZoneInfo timeZone)
        {
            string id = timeZone.Id.ToLower();

            if (id.Contains("london") || id.Contains("gmt")) return "GB";
            if (id.Contains("berlin") || id.Contains("paris") || id.Contains("central europe")) return "FR";
            if (id.Contains("w. central africa") || id.Contains("lagos")) return "NG";
            if (id.Contains("tokyo")) return "JP";
            if (id.Contains("eastern standard time") || id.Contains("new york")) return "US";

            // Default fallback
            return new RegionInfo(CultureInfo.CurrentCulture.LCID).TwoLetterISORegionName;
        }

        // Helper method to convert price
        public static decimal ConvertPrice(decimal amountNaira, string regionCode, out string currencySymbol)
        {
            // Dummy conversion rates relative to Naira
            var rates = new Dictionary<string, (decimal Rate, string Symbol)>
            {
                { "NG", (1m, "#") },      // Nigeria
                { "US", (0.0012m, "$") }, // Dollar
                { "GB", (0.00095m, "£") },// Pound
                { "FR", (0.0011m, "€") }, // Euro
                { "JP", (0.18m, "¥") }    // Yen
            };

            if (!rates.ContainsKey(regionCode))
            {
                regionCode = "US"; // default fallback
            }

            var (rate, symbol) = rates[regionCode];
            currencySymbol = symbol;

            return amountNaira * rate;
        }

        // Helper method to reset all data
        public static void ResetAllData(IRepository<Room> roomRepo, IRepository<Customer> customerRepo, IRepository<Booking> bookingRepo)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

            if (Directory.Exists(folder))
            {
                foreach (var file in Directory.GetFiles(folder, "*.json"))
                {
                    File.Delete(file);
                }
                Console.WriteLine("✅ All JSON files deleted.");
            }

            // Call ClearAll() on each repo to reset memory
            (roomRepo as InMemoryRepository<Room>)?.ClearAll();
            (customerRepo as InMemoryRepository<Customer>)?.ClearAll();
            (bookingRepo as InMemoryRepository<Booking>)?.ClearAll();

            Console.WriteLine("✅ All in-memory repositories cleared.");
        }


        // Helper method to supply preset data
        public static void SupplyPresetData(IRepository<Room> roomRepo)
        {
            var roomTypes = new[]
            {
                new { Type = "Presidential", Price = 100000m, Count = 1 }, // top tier, very rare
                new { Type = "Executive", Price = 60000m, Count = 2 },    // new higher tier
                new { Type = "Suite", Price = 40000m, Count = 3 },
                new { Type = "Deluxe", Price = 25000m, Count = 4 },
                new { Type = "Standard", Price = 15000m, Count = 5 },     // more common
            };

            foreach (var rt in roomTypes)
            {
                for (int i = 0; i < rt.Count; i++)
                {
                    roomRepo.Add(new Room { Type = rt.Type, PricePerNight = rt.Price, IsBooked = false });
                }
            }

            Console.WriteLine("✅ Preset data has been added successfully.");
        }
    }
}
