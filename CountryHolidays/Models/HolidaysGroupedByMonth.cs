using System.Collections.Generic;

namespace CountryHolidays.Models
{
    public class HolidaysGroupedByMonth
    {
        public int Month { get; set; }
        public List<Holiday> Holidays { get; set; }
    }
}