using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace C_Project
{
    public class CurrencyService
    {
        private Dictionary<string, decimal> _rates = new Dictionary<string, decimal>();

        public async Task UpdateRatesAsync()
        {
            // Simulated rates (1 NGN = ? target currency)
            _rates["USD"] = 0.0011m;  // 1 NGN = $0.0011
            _rates["GBP"] = 0.00085m; // 1 NGN = £0.00085
            _rates["EUR"] = 0.0010m;  // 1 NGN = €0.0010
            _rates["JPY"] = 0.16m;    // 1 NGN = ¥0.16
            _rates["NGN"] = 1m;       // Base currency
        }

        public decimal GetRate(string fromCurrency, string toCurrency)
        {
            if (!_rates.ContainsKey(fromCurrency) || !_rates.ContainsKey(toCurrency))
                return 1m;

            // Since rates are stored as NGN → target, just invert if needed
            if (fromCurrency == "NGN")
                return _rates[toCurrency];

            if (toCurrency == "NGN")
                return 1 / _rates[fromCurrency];

            // Convert via NGN as intermediate
            return _rates[toCurrency] / _rates[fromCurrency];
        }
    }

    //public class ExchangeRateResponse
    //{
    //    public Dictionary<string, decimal> Rates { get; set; } = new();

    //}
}
