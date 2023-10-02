using System;
using System.Collections.Generic;
using HotelBooking.Core;

namespace HotelBooking.UnitTests.Fakes;

public class TestDataGenerator
{
    public static IEnumerable<object[]> GetAvailableRoomData()
    {
        var data = new List<object[]>
        {
            new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 },
            new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(4), -1 },
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

    public static IEnumerable<object[]> CreateBookingRoomAvailableData()
    {
        var data = new List<object[]>()
        {
            new object[] { new Booking { 
                Id = 1,
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(3),
                IsActive = false,
                CustomerId = 1,
                Customer = new Customer(),
                RoomId = 1,
                Room = new Room(),
            }, true },
            new object[] { new Booking { 
                Id = 2,
                StartDate = DateTime.Today.AddDays(8),
                EndDate = DateTime.Today.AddDays(9),
                IsActive = false,
                CustomerId = 2,
                Customer = new Customer(),
                RoomId = 2,
                Room = new Room(),
            }, true }
        };
        return data;
    }
}