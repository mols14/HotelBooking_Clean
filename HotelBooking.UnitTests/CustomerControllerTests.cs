using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.WebApi.Controllers;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests;

public class CustomerControllerTests
{
    private CustomersController controller;
    private Mock<IRepository<Customer>> fakeCustomerRepository;
    private readonly List<Customer> customers;

    public CustomerControllerTests()
    {
        customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Mathias", Email = "mathias@mail.dk" },
            new Customer { Id = 2, Name = "Anne", Email = "anne@mail.dk" },
        };

        // Create fake Customer repo
        fakeCustomerRepository = new Mock<IRepository<Customer>>();
        
        // Implement fake methods
        fakeCustomerRepository.Setup(x => x.GetAll()).Returns(customers);
        
        // Create CustomerController
        controller = new CustomersController(fakeCustomerRepository.Object);
    }

    [Fact]
    public void GetAll_ReturnsListWithCorrectNumberOfCustomers()
    {
        // Act
        var result = controller.Get() as List<Customer>;
        var noOfCustomers = result.Count;
        
        // Assert
        Assert.Equal(2, noOfCustomers);
    }

    [Fact]
    public void GetAll_CallsRepoExactlyOnce()
    {
        // Act
        controller.Get();
        
        // Verify
        fakeCustomerRepository.Verify(x => x.GetAll(), Times.Once);
    }
}