using System;
using System.Collections.Generic;
using HotelBooking.Core;

namespace HotelBooking.IntegrationTests.Utils;

public class TestDataGenerator
{
    public static IEnumerable<object[]> CreateBookingRoomAvailable()
    {
        var date = new List<object[]>()
        {
            new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(2), new List<int> { 1, 2, 3 } }
        };
        return date;
    }
}