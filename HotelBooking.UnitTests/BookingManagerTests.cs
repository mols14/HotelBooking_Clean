using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;


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
        [ClassData(typeof(AvailableRoomData))]
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
    }
}
