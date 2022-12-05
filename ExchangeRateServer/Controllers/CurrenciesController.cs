using ExchangeRateServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using System.Diagnostics;

namespace ExchangeRateServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Rate>> Get(string currencyName, DateTime startDate, DateTime endDate)
        {
            List<Rate> rates = await LocalCache.GetRatesByCurrencyNameWithDateRange(currencyName, startDate, endDate);

            return rates;
        }
    }
}
