using Shared.Data;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ExchangeRateServer.Services
{
    public class LocalCache
    {
        private static readonly string StorageFile = $"{Directory.GetCurrentDirectory()}/Storage/rates.json";

        private static HttpClient _client = new HttpClient();
        private static List<Rate> _rates = new List<Rate>();

        static LocalCache()
        {
            LoadRatesFromFile();
        }

        private static void LoadRatesFromFile()
        {
            if (!File.Exists(StorageFile))
            {
                File.Create(StorageFile).Close();
            }
            else
            {
                string content = File.ReadAllText(StorageFile);
                _rates = JsonConvert.DeserializeObject<List<Rate>>(content) ?? new List<Rate>(); 
            }
        }
        
        private static void SaveRatesToFile()
        {
            string serialized = JsonConvert.SerializeObject(_rates, Formatting.Indented);
            File.WriteAllText(StorageFile, serialized);
        }

        public static List<Rate> GetRatesByCurrencyName(string currencyName)
        {
            return _rates.Where(r => r.Currency == currencyName).OrderBy(r => r.Date).ToList();
        }

        public static async Task<List<Rate>> GetRatesByCurrencyNameWithDateRange(string currencyName, DateTime startDate, DateTime endDate)
        {
            var rates = new List<Rate>();
            Rate rate;

            DateTime tempDate = startDate;

            while (tempDate <= endDate)
            {
                rate = await GetRateByCurrencyNameWithDate(currencyName, tempDate);
                rates.Add(rate);
                tempDate = tempDate.AddDays(1);
            }

            return rates;
        }

        public static async Task<Rate> GetRateByCurrencyNameWithDate(string currencyName, DateTime date)
        {
            var result = _rates.FirstOrDefault(r => r.Currency.ToUpper() == currencyName.ToUpper() && r.Date == date);

            if (result != null)
            {
                return result;
            }

            var content = await _client.GetStringAsync($"https://www.nbrb.by/api/exrates/rates/{currencyName}?ondate={date.ToString("yyyy-MM-dd")}&parammode=2");
            var rateApi = JsonConvert.DeserializeObject<RateApi>(content);
            var rate = new Rate { Currency = rateApi.Cur_Abbreviation, Date = rateApi.Date, Amount = rateApi.Cur_Scale, Value = rateApi.Cur_OfficialRate };

            _rates.Add(rate);
            SaveRatesToFile();

            return rate;
        }
    }
}
