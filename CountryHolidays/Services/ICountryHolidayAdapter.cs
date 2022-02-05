using CountryHolidays.Models;
using CountryHolidays.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryHolidays.Services
{
    public interface ICountryHolidayAdapter
    {
        public Task<List<Country>> GetCountryList();
        public Task<List<Holiday>> GetCountryHolidays(int year, string country);
        public Task<IsPublicHolidayResponseModel> IsPublicHoliday(DateTime date, string country);
    }
}
