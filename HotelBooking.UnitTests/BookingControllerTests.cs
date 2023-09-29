using System;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.Core;
using HotelBooking.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace HotelBooking.UnitTests;

public class BookingControllerTests
{
    private BookingsController controller;
    private Mock<IRepository<Booking>> fakeBookingsRepository;
    private Mock<IRepository<Customer>> fakeCustomerRepo;
    private Mock<IRepository<Room>> fakeRoomRepo;
    private Mock<IBookingManager> fakeBookingManager;
    private readonly List<Booking> bookings;
    private readonly ITestOutputHelper output;

    public BookingControllerTests(ITestOutputHelper output)
    {
        this.output = output;
        var newBooking = new Booking
        {
            Id = 3, Customer = new Customer(), CustomerId = 3, Room = new Room(), RoomId = 3,
            StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2), IsActive = true
        };
        
        bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1, Customer = new Customer(), CustomerId = 1, StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1), IsActive = true, Room = new Room(), RoomId = 1
            },
            new Booking
            {
                Id = 2, Customer = new Customer(), CustomerId = 2, StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1), IsActive = true, Room = new Room(), RoomId = 2
            },
        };
        
        // Create fake Booking repo
        fakeBookingsRepository = new Mock<IRepository<Booking>>();
        fakeCustomerRepo = new Mock<IRepository<Customer>>();
        fakeRoomRepo = new Mock<IRepository<Room>>();
        fakeBookingManager = new Mock<IBookingManager>();
        
        // Implement fake methods for booking repo
        fakeBookingsRepository.Setup(x => x.Get(It.IsAny<int>()))
            .Returns((int i) => bookings.FirstOrDefault(r => r.Id == i));

        fakeBookingsRepository.Setup(x => x.GetAll()).Returns(bookings);
        
        // Implement fake methods for booking manager
        fakeBookingManager.Setup(x => x.CreateBooking(It.IsAny<Booking>())).Returns(true).Callback(() =>
        {
            bookings.Add(newBooking);
        });
        
        controller = new BookingsController(fakeBookingsRepository.Object, fakeRoomRepo.Object, fakeCustomerRepo.Object, fakeBookingManager.Object);
    }

    [Fact]
    public void GetAll_ReturnsListWithCorrectNumberOfBookings()
    {
        // Act
        var result = controller.Get() as List<Booking>;
        var noOfBookings = result?.Count;
        
        // Assert
        Assert.Equal(2, noOfBookings);
    }

    [Fact]
    public void GetAll_CallsRepoExactlyOnce()
    {
        // Act
        controller.Get();
        
        //Verify
        fakeBookingsRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_BookingExists_ReturnsIActionResultWithBooking()
    {
        // Act
        var result = controller.Get(1) as ObjectResult;
        var booking = result.Value as Booking;
        var bookingId = booking.Id;
        
        // Assert
        Assert.InRange(bookingId, 1, 2);
    }

    [Fact]
    public void GetById_RoomExists_CallsRepoOnce()
    {
        // Arrange
        var bookingId = 1;
        
        // Act
        controller.Get(bookingId);
        
        // Verify
        fakeBookingsRepository.Verify(x => x.Get(bookingId), Times.Once);
    }

    [Fact]
    public void GetById_BookingDoesntExists_ReturnsNotFound()
    {
        // Arrange
        var bookingId = 3;
        
        // Act
        var result = controller.Get(bookingId);
        var notFoundResult = result as NotFoundResult;
        
        //Assert
        Assert.Equal(404, notFoundResult?.StatusCode);
    }

    [Fact]
    public void Post_WithBooking_ReturnsNewRoute()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 3, Customer = new Customer(), CustomerId = 2, Room = new Room(), RoomId = 2,
            StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true
        };
        
        // Act
        var result = controller.Post(booking) as CreatedAtRouteResult;
        
        // Assert
        Assert.Equal("GetBookings", result?.RouteName);
    }

    [Fact]
    public void Post_WithBooking_AddsBookingToTheBookingList()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 3, Customer = new Customer(), CustomerId = 3, Room = new Room(), RoomId = 3,
            StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true
        };
        
        // Act
        controller.Post(booking);
        
        // Assert
        Assert.Equal(3, bookings.Count);
    }

    [Fact]
    public void Post_WithNoBooking_ReturnsBadRequest()
    {
        // Act
        var result = controller.Post(null);
        var badRequestResult = result as BadRequestResult;
        
        // Assert
        Assert.Equal(400, badRequestResult?.StatusCode);
    }

    [Fact]
    public void Post_WithBooking_CallsManagerOnce()
    {
        var booking = new Booking
                {
                    Id = 3, Customer = new Customer(), CustomerId = 3, Room = new Room(), RoomId = 3,
                    StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true
                };
        // Act
        controller.Post(booking);
        
        // Verify
        fakeBookingManager.Verify(x => x.CreateBooking(booking), Times.Once);
    }

    [Fact]
    public void Put_WithIdAndBooking_ReturnsNoContent()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1, Customer = new Customer(), CustomerId = 2, Room = new Room(), RoomId = 2,
            StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true
        };
        
        // Act
        var result = controller.Put(1 ,booking) as NoContentResult;
        
        // Assert
        Assert.Equal(204, result?.StatusCode);
    }

    [Fact]
    public void Put_IdBodyAndBookingIdDoesntMatch_ReturnsBadRequest()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1, Customer = new Customer(), CustomerId = 2, Room = new Room(), RoomId = 2,
            StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true
        };
        
        // Act
        var result = controller.Put(2 ,booking) as BadRequestResult;
        
        // Assert
        Assert.Equal(400, result?.StatusCode);
    }

    [Fact]
    public void Put_BookingIsNull_ReturnsBadRequest()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1, Customer = new Customer(), CustomerId = 2, Room = new Room(), RoomId = 2,
            StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true
        };
        
        // Act
        var result = controller.Put(1 ,null) as BadRequestResult;
        
        // Assert
        Assert.Equal(400, result?.StatusCode);
    }

    [Fact]
    public void Delete_WithId_ReturnsNoContent()
    {
        // Act
        var result = controller.Delete(1) as NoContentResult;
        
        // Assert
        Assert.Equal(204, result?.StatusCode);
    }

    [Fact]
    public void Delete_IdNotValid_ReturnsNotFound()
    {
        // Act
        var result = controller.Delete(4 ) as NotFoundResult;
        
        // Assert
        Assert.Equal(404, result?.StatusCode);
    }
}