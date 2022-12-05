using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using Newtonsoft.Json;
using Shared.Data;
using ExchangeRateClient.Utils;

namespace ExchangeRateClient.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<Rate>?> GetCurrencyRates(string? currencyName, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(currencyName))
            {
                throw new ArgumentNullException("Название валюты не может быть пустым или null-значением");
            }

            string formattedStartDate = startDate.ToString("yyyy-MM-dd");
            string formattedEndDate = endDate.ToString("yyyy-MM-dd");

            string requestUrl = $"https://localhost:7176/api/currencies?currencyName={currencyName}&startDate={formattedStartDate}&endDate={formattedEndDate}";

            List<Rate>? rates = new List<Rate>();

            try
            {
                var responseBody = await _httpClient.GetStringAsync(requestUrl);

                rates = JsonConvert.DeserializeObject<List<Rate>>(responseBody);

                if (rates is null)
                {
                    throw new NullReferenceException("Невозможно десериализовать полученные данные курса валюты");
                }
            }
            catch (NullReferenceException e)
            {
                CommonUtils.ErrorMessageBoxShow(e.Message);
            }

            catch
            {
                CommonUtils.ErrorMessageBoxShow("Невозможно получить данные курса {currencyName}. Проверьте работоспособность сервера или адрес отправляемого запроса");
            }

            return rates;
        }
    }
}
