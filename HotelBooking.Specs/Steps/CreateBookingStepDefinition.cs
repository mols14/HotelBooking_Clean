using HotelBooking.Core;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace HotelBooking.Specs.Steps;

[Binding]
public class CreateBookingStepDefinition
{
    private BookingManager _bookingManager;
    private Mock<IRepository<Booking>> _bookingMock;
    private Mock<IRepository<Room>> _roomMock;
    private DateTime fullyOccupiedStartDate;
    private DateTime fullyOccupiedEndDate;
    private DateTime startDate;
    private DateTime endDate;
    private bool canBookRoom;

    public CreateBookingStepDefinition(ScenarioContext scenarioContext)
    {
        var rooms = new List<Room>
        {
            new Room { Id=1, Description="A" },
            new Room { Id=2, Description="B" },
        };
        var bookings = new List<Booking>
        {
            new Booking { StartDate=DateTime.Now.AddDays(5), EndDate=DateTime.Now.AddDays(10), RoomId=1, IsActive=false},
            new Booking { StartDate=DateTime.Now.AddDays(11), EndDate=DateTime.Now.AddDays(15), RoomId=1, IsActive=false},
            new Booking { StartDate=DateTime.Parse("2023-12-02"), EndDate=DateTime.Parse("2023-12-07"), RoomId=1, IsActive=true},
            new Booking { StartDate=DateTime.Now.AddDays(5), EndDate=DateTime.Now.AddDays(10), RoomId=2, IsActive=false},
            new Booking { StartDate=DateTime.Now.AddDays(11), EndDate=DateTime.Now.AddDays(15), RoomId=2, IsActive=false},
            new Booking { StartDate=DateTime.Parse("2023-12-02"), EndDate=DateTime.Parse("2023-12-07"), RoomId=2, IsActive=true},
        };

        // Create fake Repositories. 
        _roomMock = new Mock<IRepository<Room>>();
        _bookingMock = new Mock<IRepository<Booking>>();

        // Implement fake GetAll() method.
        _roomMock.Setup(x => x.GetAll()).Returns(rooms);
        _bookingMock.Setup(x => x.GetAll()).Returns(bookings);

        _bookingManager = new BookingManager(_bookingMock.Object, _roomMock.Object);
    }

    [Given(@"the fully occupied range is from ""(.*)"" to ""(.*)""")]
    public void GivenTheFullyOccupiedRangeIsFromTo(string startDate, string endDate)
    {
        fullyOccupiedStartDate = DateTime.Parse(startDate);
        fullyOccupiedEndDate = DateTime.Parse(endDate);
    }

    [When(@"I request to create a booking with SD ""(.*)"" and ED ""(.*)""")]
    public void WhenIRequestToCreateABookingWithSDAndED(string sd, string ed)
    {
        startDate = DateTime.Parse(sd);
        endDate = DateTime.Parse(ed);
        var booking = new Booking
        {
            StartDate = startDate,
            EndDate = endDate
        };

        _bookingManager.CreateBooking(booking);
    }

    [Then(@"the booking should be successful")]
    public void ThenTheBookingShouldBeSuccessful()
    {
        Assert.True(canBookRoom);
    }

    [Then(@"the booking should fail")]
    public void ThenTheBookingShouldFail()
    {
        Assert.False(canBookRoom);
    }
}