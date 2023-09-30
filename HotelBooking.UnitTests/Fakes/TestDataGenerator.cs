using System;
using System.Collections;
using System.Collections.Generic;

namespace HotelBooking.UnitTests.Fakes;

public class TestDataGenerator
{
    public static IEnumerable<object[]> GetAvailableRoomData()
    {
        var data = new List<object[]>
        {
            new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 },
            new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(4), -1 },
            new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 },
            new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 },
            new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 },
        };
        return data;
    }

    public static IEnumerable<object[]> GetStartDateLaterThanEndDateData()
    {
        var data = new List<object[]>()
        {
            new object[] { DateTime.Today.AddDays(3), DateTime.Today.AddDays(2) },
            new object[] { DateTime.Today.AddDays(7), DateTime.Today.AddDays(2) },
            new object[] { DateTime.Today.AddDays(13), DateTime.Today.AddDays(9) },
            new object[] { DateTime.Today.AddDays(20), DateTime.Today.AddDays(9) },

        };
        return data;
    }

    public static IEnumerable<object[]> FullyOccupiedCorrectDatesData()
    {
        var data = new List<object[]>()
        {
            new object[] { DateTime.Today.AddDays(5), DateTime.Today.AddDays(15) },
            new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(12) },
            new object[] { DateTime.Today.AddDays(3), DateTime.Today.AddDays(6) },
        };
        return data;
    }

    public static IEnumerable<object[]> FullyOccupiedDatesLengthData()
    {
        var data = new List<object[]>()
        {
            new object[] { DateTime.Today.AddDays(5), DateTime.Today.AddDays(15), 6 },
            new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(12), 3 },
            new object[] { DateTime.Today.AddDays(3), DateTime.Today.AddDays(6), 0 },
            new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20), 11 },
        };
        return data;
    }

    public static IEnumerable<object[]> FullyOccupiedDatesData()
    {
        var data = new List<object[]>()
        {
            new object[] { DateTime.Today.AddDays(7), DateTime.Today.AddDays(9), GetDateTimesBetweenTwoDates(DateTime.Today.AddDays(7), DateTime.Today.AddDays(9)) },
            new object[] { DateTime.Today.AddDays(7), DateTime.Today.AddDays(12), GetDateTimesBetweenTwoDates(DateTime.Today.AddDays(7), DateTime.Today.AddDays(12)) },
            new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(12), GetDateTimesBetweenTwoDates(DateTime.Today.AddDays(10), DateTime.Today.AddDays(12)) },
            new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20), GetDateTimesBetweenTwoDates(DateTime.Today.AddDays(10), DateTime.Today.AddDays(20)) },
        };
        return data;
    }

    private static List<DateTime> GetDateTimesBetweenTwoDates(DateTime start, DateTime end)
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