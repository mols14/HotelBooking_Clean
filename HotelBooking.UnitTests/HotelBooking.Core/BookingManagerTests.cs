using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using HotelBooking.Infrastructure.Repositories;
using HotelBooking.UnitTests.Utils;
using HotelBooking.WebApi.Controllers;
using Moq;
using Xunit.Abstractions;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private Mock<IRepository<Booking>> fakeBookingRepository;
        private Mock<IRepository<Room>> fakeRoomRepository;
        private readonly List<Booking> bookings;
        private readonly List<Room> rooms;
        private readonly ITestOutputHelper output;
        
        public BookingManagerTests(ITestOutputHelper output)
        {
            this.output = output;
            var newBooking = new Booking
            {
                Id = 3, Customer = new Customer(), CustomerId = 3, Room = new Room(), RoomId = 3,
                StartDate = DateTime.Today, EndDate = DateTime.Now.AddDays(2), IsActive = true
            };
        
            bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1, Customer = new Customer(), CustomerId = 1, StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1), IsActive = false, Room = new Room(), RoomId = 1
                },
                new Booking
                {
                    Id = 2, Customer = new Customer(), CustomerId = 2, StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1), IsActive = false, Room = new Room(), RoomId = 2
                },
                new Booking
                {
                    Id = 3, Customer = new Customer(), CustomerId = 3, StartDate = DateTime.Today.AddDays(10),
                    EndDate = DateTime.Today.AddDays(20), IsActive = true, Room = new Room(), RoomId = 3
                },
            };
            
            rooms = new List<Room>
            {
                new Room { Id = 1, Description = "A" },
                new Room { Id = 2, Description = "B" },
                new Room { Id = 3, Description = "C" },
            };
            
            // Create fake booking repo
            fakeBookingRepository = new Mock<IRepository<Booking>>();
            fakeRoomRepository = new Mock<IRepository<Room>>();
            
            // Implement fake methods for booking repo
            fakeBookingRepository.Setup(x => x.Add(It.IsAny<Booking>())).Callback(() =>
            {
                bookings.Add(newBooking);
            });

            fakeBookingRepository.Setup(x => x.Edit(It.IsAny<Booking>()));

            fakeBookingRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(newBooking);

            fakeBookingRepository.Setup(x => x.GetAll()).Returns(bookings);

            fakeBookingRepository.Setup(x => x.Remove(It.IsAny<int>())).Callback(() =>
            {
                bookings.RemoveAt(1);
            });
            
            // Implement fake methods for room repo
            fakeRoomRepository.Setup(x => x.GetAll()).Returns(rooms);
            
            bookingManager = new BookingManager(fakeBookingRepository.Object, fakeRoomRepository.Object);
        }

        [Fact]
        public void CreateBooking_WithBooking_CallsBookingRepositoryAdd()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 3,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2),
                IsActive = false,
                CustomerId = 1,
                RoomId = 1,
                Customer = new Customer(),
                Room = new Room()
            };
            // Act
            bookingManager.CreateBooking(booking);
            // Assert
            fakeBookingRepository.Verify(x => x.Add(booking), Times.Once());
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.CreateBookingRoomAvailableData), MemberType = typeof(TestDataGenerator))]
        public void CreateBooking_RoomAvailable_ReturnsTrue(Booking booking, bool expected)
        {
            output.WriteLine("enddate "+ booking.EndDate);
            // Act
            var result = bookingManager.CreateBooking(booking);
            // Assert
            Assert.Equal(expected, result);
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

        // [Fact]
        // public void FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        // {
        //     // This test was added to satisfy the following test design
        //     // principle: "Tests should have strong assertions".
        //
        //     // Arrange
        //     DateTime date = DateTime.Today.AddDays(1);
        //     // Act
        //     int roomId = bookingManager.FindAvailableRoom(date, date);
        //
        //     // Assert
        //     var bookingForReturnedRoomId = fakeBookingRepository.GetAll().Where(
        //         b => b.RoomId == roomId
        //         && b.StartDate <= date
        //         && b.EndDate >= date
        //         && b.IsActive);
        //
        //     Assert.Empty(bookingForReturnedRoomId);
        // }

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
            output.WriteLine("dates: " + occupiedDates.Count);
            // Assert
            Assert.IsType<List<DateTime>>(occupiedDates);
        }
    }
}
