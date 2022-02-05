using CountryHolidays.Models;
using CountryHolidays.Models.RequestModels;
using CountryHolidays.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CountryHolidays.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryHolidaysController : ControllerBase
    {
        private readonly ILogger<CountryHolidaysController> _logger;
        private readonly ICountryHolidayAdapter _countryHolidayAdapter;

        public CountryHolidaysController(ILogger<CountryHolidaysController> logger, ICountryHolidayAdapter countryHolidayAdapter)
        {
            _logger = logger;
            _countryHolidayAdapter = countryHolidayAdapter;
        }

        [HttpGet("CountryList")]
        public async Task<List<Country>> GetCountryListAsync()
        {
            return await _countryHolidayAdapter.GetCountryList();
        }

        [HttpGet("CountryGroupedHolidays")]
        public async Task<List<HolidaysGroupedByMonth>> GetCountryHolidaysAsync([FromQuery] GetCountryHolidays request)
        {
            var holidaysList = await _countryHolidayAdapter.GetHolidays(request.Year, request.Country);

            var groupedHolidays = holidaysList.GroupBy(x => x.Date.Month).Select(x => new HolidaysGroupedByMonth
            {
               Month = x.Key,
               Holidays = x.ToList()
            }).ToList();

            return groupedHolidays;
        }
    }
}
