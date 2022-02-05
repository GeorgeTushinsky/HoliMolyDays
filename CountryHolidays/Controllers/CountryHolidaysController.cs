using CountryHolidays.Models;
using CountryHolidays.Models.RequestModels;
using CountryHolidays.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CountryHolidays.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryHolidaysController : ControllerBase
    {
        private readonly ILogger<CountryHolidaysController> _logger;
        private readonly ICountryHolidayAdapter _countryHolidayAdapter;
        private readonly IDistributedCache _cache;
        private const int CACHE_TIME = 24 * 60 * 60;

        public CountryHolidaysController(
            ILogger<CountryHolidaysController> logger,
            ICountryHolidayAdapter countryHolidayAdapter,
            IDistributedCache cache)
        {
            _logger = logger;
            _countryHolidayAdapter = countryHolidayAdapter;
            _cache = cache;
        }

        [HttpGet("CountryList")]
        public async Task<List<Country>> GetCountryListAsync(CancellationToken cancellationToken)
        {
            var cached = await _cache.GetStringAsync("CountryList", cancellationToken);
            List<Country> result = new List<Country>();

            if (cached == null)
            {
                result = await _countryHolidayAdapter.GetCountryList();
                var json = JsonSerializer.Serialize(result);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_TIME));
                await _cache.SetStringAsync("CountryList", json, options, cancellationToken);

                return result;
            }

            result = JsonSerializer.Deserialize<List<Country>>(cached);
            return result;
        }

        [HttpGet("CountryGroupedHolidays")]
        public async Task<List<HolidaysGroupedByMonth>> GetCountryHolidaysAsync([FromQuery] GetCountryHolidays request, CancellationToken cancellationToken)
        {
            var cacheKey = $"CountryGroupedHolidays/{request.Year}/{request.Country}";
            var cached = await _cache.GetStringAsync(cacheKey);

            if (cached == null)
            {
                var holidaysList = await _countryHolidayAdapter.GetCountryHolidays(request.Year, request.Country);

                var groupedHolidays = holidaysList
                    .GroupBy(x => x.Date.Month)
                    .Select(x => new HolidaysGroupedByMonth
                    {
                        Month = x.Key,
                        Holidays = x.ToList()
                    }).ToList();

                var json = JsonSerializer.Serialize(groupedHolidays);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_TIME));
                await _cache.SetStringAsync(cacheKey, json, options, cancellationToken);

                return groupedHolidays;
            }

            return JsonSerializer.Deserialize<List<HolidaysGroupedByMonth>>(cached);
        }

        [HttpGet("DayStatus")]
        public async Task<DayStatus> GetDayStatus([FromQuery] GetDayStatus request, CancellationToken cancellationToken)
        {
            var cacheKey = $"DayStatus/{request.Date}/{request.Country}";
            var cache = await _cache.GetStringAsync(cacheKey);

            if (cache == null)
            {
                var isPublicHoliday = await _countryHolidayAdapter.IsPublicHoliday(request.Date, request.Country);

                var result = new DayStatus
                {
                    Status = isPublicHoliday.IsPublicHoliday ? "PUBLIC_HOLIDAY" : "WORK_DAY"
                };

                var json = JsonSerializer.Serialize(isPublicHoliday);

                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_TIME));
                await _cache.SetStringAsync(cacheKey, json, options, cancellationToken);
                return result;
            }

            return JsonSerializer.Deserialize<DayStatus>(cache);
        }

        [HttpGet("CountryMaximumHolidaysInRow")]
        public async Task<int> GetCountryMaximumHolidaysInRow([FromQuery] GetCountryHolidays request, CancellationToken cancellationToken)
        {
            var cacheKey = $"CountryMaximumHolidaysInRow/{request.Year}/{request.Country}";
            var cache = await _cache.GetStringAsync(cacheKey);

            if (cache == null)
            {
                var holidays = await _countryHolidayAdapter.GetCountryHolidays(request.Year, request.Country);
                holidays = holidays.OrderBy(x => x.Date.Month).ThenBy(x => x.Date.Day).ToList();
                var result = 0;
                var counter = 0;

                for (int i = 0; i < holidays.Count - 1; i++)
                {
                    var holiday = holidays[i].Date;
                    var nextHoliday = holidays[i + 1].Date;
                    var timeSpan = new DateTime(nextHoliday.Year, nextHoliday.Month, nextHoliday.Day) - new DateTime(holiday.Year, holiday.Month, holiday.Day);

                    if (timeSpan.Days <= 1)
                    {
                        counter++;
                        result = counter > result ? counter : result;
                    }
                    else
                    {
                        counter = 1;
                    }
                }

                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_TIME));
                await _cache.SetStringAsync(cacheKey, result.ToString(), options, cancellationToken);

                return result;
            }

            return Convert.ToInt32(cache);
        }
    }
}
