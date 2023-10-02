using System;
using System.Collections.Generic;

namespace HotelBooking.IntegrationTests.Utils;

public class OccupiedDates
{
    public static List<DateTime> GetDateTimesBetweenTwoDates(DateTime start, DateTime end)
    {
        var dates = new List<DateTime>();
        for (var dt = start; dt <= end; dt = dt.AddDays(1))
        {
            if (dt >= DateTime.Today.AddDays(10) && dt <= DateTime.Today.AddDays(20))
            {
                dates.Add(dt);
            }
        }
        return dates;
    }
}