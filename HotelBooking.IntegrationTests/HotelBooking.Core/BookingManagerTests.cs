using System;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.Core;
using HotelBooking.Infrastructure;
using HotelBooking.Infrastructure.Repositories;
using HotelBooking.IntegrationTests.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotelBooking.IntegrationTests
{
    public class BookingManagerTests : IDisposable
    {
        // This test class uses a separate Sqlite in-memory database. While the
        // .NET Core built-in in-memory database is not a relational database,
        // Sqlite in-memory database is. This means that an exception is thrown,
        // if a database constraint is violated, and this is a desirable behavior
        // when testing.

        SqliteConnection connection;
        BookingManager bookingManager;
        private readonly BookingRepository bookingRepos;

        public BookingManagerTests()
        {
            connection = new SqliteConnection("DataSource=:memory:");

            // In-memory database only exists while the connection is open
            connection.Open();

            // Initialize test database
            var options = new DbContextOptionsBuilder<HotelBookingContext>()
                            .UseSqlite(connection).Options;
            var dbContext = new HotelBookingContext(options);
            IDbInitializer dbInitializer = new DbInitializer();
            dbInitializer.Initialize(dbContext);

            // Create repositories and BookingManager
            bookingRepos = new BookingRepository(dbContext);
            var roomRepos = new RoomRepository(dbContext);
            bookingManager = new BookingManager(bookingRepos, roomRepos);
        }

        public void Dispose()
        {
            // This will delete the in-memory database
            connection.Close();
        }
        
        [Fact]
        public void FindAvailableRoom_RoomNotAvailable_RoomIdIsMinusOne()
        {
            // Act
            var roomId = bookingManager.FindAvailableRoom(DateTime.Today.AddDays(8), DateTime.Today.AddDays(8));
            // Assert
            Assert.Equal(-1, roomId);
        }
        
        [Theory]
        [MemberData(nameof(TestDataGenerator.CreateBookingRoomAvailable), MemberType = typeof(TestDataGenerator))]
        public void FindAvailableRoom_RoomAvailable_ReturnsRoomId(DateTime start, DateTime end, List<int> expected)
        {
            // Act
            var roomId = bookingManager.FindAvailableRoom(start, end);
            // Assert
            Assert.InRange(roomId, expected[0], expected[2]);
        }
        
        [Fact]
        public void CreateBooking_WithBooking_AddsBookingToDb()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            var booking = new Booking
            {
                StartDate = date,
                EndDate = date.AddDays(2),
                IsActive = true,
                CustomerId = 1,
                RoomId = 3
            };
            // Act
            bookingManager.CreateBooking(booking);
            // Assert
            Assert.Equal(4, bookingRepos.GetAll().Count());
        }

        [Fact]
        public void CreateBooking_OccupiedRoom_DoesNotAddBooking()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            var booking = new Booking
            {
                StartDate=date,
                EndDate=date.AddDays(14),
                IsActive=false,
                CustomerId=1,
                RoomId=1
            };
            // Act
            bookingManager.CreateBooking(booking);
            // Assert
            Assert.Equal(3, bookingRepos.GetAll().Count());
        }

        [Fact]
        public void GetFullyOccupiedDates_WithDates_ReturnsOccupiedDates()
        {
            // Arrange
            var start = DateTime.Today.AddDays(10);
            var end = DateTime.Today.AddDays(15);
            // Act
            var result = bookingManager.GetFullyOccupiedDates(start, end);
            // Assert
            Assert.Equal(OccupiedDates.GetDateTimesBetweenTwoDates(start,end), result);
        }
    }
}
