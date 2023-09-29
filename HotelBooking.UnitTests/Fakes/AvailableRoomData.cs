using System;
using System.Collections;
using System.Collections.Generic;

namespace HotelBooking.UnitTests.Fakes;

public class AvailableRoomData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(4), -1 };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 };
        yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1), -1 };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}