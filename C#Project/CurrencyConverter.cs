using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Project
{
    public static class CurrencyConverterExtensions
    {
        //public static decimal ConvertToLocalCurrency(this decimal amountInNGN, TimeZoneInfo userTimeZone, CurrencyService currencyService)
        //{
        //    string regionCode = Program.GetRegionCodeFromTimeZone(userTimeZone);
        //    var region = new RegionInfo(regionCode);

        //    // Get currency code (e.g. "USD", "EUR", "JPY", etc.)
        //    string targetCurrency = region.ISOCurrencySymbol;

        //    // Use currencyService to get conversion
        //    decimal rate = currencyService.GetRate("NGN", targetCurrency);

        //    return amountInNGN * rate;
        //}
    }

}
