using System.Collections.Generic;

namespace CountryHolidays.Models
{
    public class Holiday
    {
        public Date Date { get; set; }
        public List<HolidayName> Name { get; set; }
        public List<HolidayNote> Note { get; set; }
        public List<string> Flags { get; set; }
        public string HolidayType { get; set; }
    }
}
