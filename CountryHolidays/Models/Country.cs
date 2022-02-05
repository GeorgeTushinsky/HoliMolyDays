using System.Collections.Generic;

namespace CountryHolidays.Models
{
    public class Country
    {
        public string CountryCode { get; set; }
        public string FullName { get; set; }
        public List<string> Regions { get; set; }
        public List<string> HolidayTypes { get; set; }
        public Date FromDate { get; set; }
        public Date ToDate { get; set; }
    }
    public class Date
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
