using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        IRepository<Booking> bookingRepository;
        
        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        [Fact]
        public void CreateBooking_RoomAvailable_CallsBookingRepositoryAdd()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                IsActive = false,
                CustomerId = 1,
                Customer = new Customer(),
                RoomId = 1,
                Room = new Room(),
            };
            // Act
            bookingManager.CreateBooking(booking);
            // Assert
            Assert.True(FakeBookingRepository.addWasCalled);
        }
        
        [Fact]
        public void CreateBooking_RoomOccupied_ReturnFalse()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                StartDate = DateTime.Today.AddDays(11),
                EndDate = DateTime.Today.AddDays(12),
                IsActive = false,
                CustomerId = 1,
                Customer = new Customer(),
                RoomId = 1,
                Room = new Room(),
            };
            // Act
            var result = bookingManager.CreateBooking(booking);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateBooking_RoomAvailable_ReturnsTrue()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(3),
                IsActive = false,
                CustomerId = 1,
                Customer = new Customer(),
                RoomId = 1,
                Room = new Room(),
            };
            // Act
            var result = bookingManager.CreateBooking(booking);
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Action act = () => bookingManager.FindAvailableRoom(date, date);
            
            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.GetAvailableRoomData), MemberType = typeof(TestDataGenerator))]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne(DateTime startDate, DateTime endDate, int expected)
        {
            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            Assert.NotEqual(expected, roomId);
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);

            // Assert
            var bookingForReturnedRoomId = bookingRepository.GetAll().Where(
                b => b.RoomId == roomId
                && b.StartDate <= date
                && b.EndDate >= date
                && b.IsActive);

            Assert.Empty(bookingForReturnedRoomId);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.GetStartDateLaterThanEndDateData), MemberType = typeof(TestDataGenerator))]
        public void GetFullyOccupiedDates_StartDateLaterThanEndDate_ThrowsArgumentException(DateTime startDate, DateTime endDate)
        {
            // Act
            Action act = () => bookingManager.GetFullyOccupiedDates(startDate, endDate);
            
            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.FullyOccupiedCorrectDatesData), MemberType = typeof(TestDataGenerator))]
        public void GetFullyOccupiedDates_CorrectDates_ReturnsListOfDateTimes(DateTime startDate, DateTime endDate)
        {
            // Act
            var occupiedDates = bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.IsType<List<DateTime>>(occupiedDates);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.FullyOccupiedDatesLengthData), MemberType = typeof(TestDataGenerator))]
        public void GetFullyOccupiedDates_OccupiedDaysShouldBeSix_ReturnsOccupiedDates(DateTime startDate, DateTime endDate, int expected)
        {
            // Act
            var occupiedDates = bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Equal(expected, occupiedDates.Count);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.FullyOccupiedDatesData), MemberType = typeof(TestDataGenerator))]
        public void GetFullyOccupiedDays_WithDates_ShouldReturnOccupiedDates(DateTime startDate, DateTime endDate, List<DateTime> occupiedDatesExpected)
        {
            // Act
            var occupiedDatesActual = bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Equal(occupiedDatesExpected, occupiedDatesActual);
        }
    }
}
