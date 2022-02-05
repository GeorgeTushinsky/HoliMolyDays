using CountryHolidays.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryHolidays.Services
{
    public interface ICountryHolidayAdapter
    {
        public Task<List<Country>> GetCountryList();
        public Task<List<Holiday>> GetHolidays(int year, string country);
    }
}
