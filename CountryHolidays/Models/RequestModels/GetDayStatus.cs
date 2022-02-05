using System;

namespace CountryHolidays.Models.RequestModels
{
    public class GetDayStatus
    {
        public DateTime Date { get; set; }
        public string Country { get; set; }
    }
}
