using Xunit;

namespace HotelBooking.Specs.Steps;

[Binding]
public class CreateBookingStepDefinition
{
    private readonly ScenarioContext _scenarioContext;
    private DateTime fullyOccupiedStartDate;
    private DateTime fullyOccupiedEndDate;
    private DateTime startDate;
    private DateTime endDate;
    private bool canBookRoom;

    public CreateBookingStepDefinition(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
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

        canBookRoom = (startDate <= fullyOccupiedEndDate && endDate >= fullyOccupiedStartDate) ||
                      (startDate > endDate);
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