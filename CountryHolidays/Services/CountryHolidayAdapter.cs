using CountryHolidays.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CountryHolidays.Services
{
    public class CountryHolidayAdapter : ICountryHolidayAdapter
    {
        private readonly HttpClient _httpClient;

        public CountryHolidayAdapter()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://kayaposoft.com/enrico/json/v2.0");
        }

        public async Task<List<Country>> GetCountryList()
        {
            var response = await _httpClient.GetAsync("?action=getSupportedCountries");

            if (response.IsSuccessStatusCode)
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(stream))
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    return new JsonSerializer().Deserialize<List<Country>>(jsonReader);
                }
            }

            throw new Exception("Something went wrong");
        }

        public async Task<List<Holiday>> GetHolidays(int year, string country)
        {
            var response = await _httpClient.GetAsync($"?action=getHolidaysForYear&year={year}&country={country}");

            if (response.IsSuccessStatusCode)
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(stream))
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    return new JsonSerializer().Deserialize<List<Holiday>>(jsonReader);
                }
            }

            throw new Exception("Something went wrong");
        }
    }
}
