using System;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.Core;
using HotelBooking.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class RoomsControllerTests
    {
        private RoomsController controller;
        private Mock<IRepository<Room>> fakeRoomRepository;
        private readonly List<Room> rooms;

        public RoomsControllerTests()
        {
            rooms = new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" },
            };

            var newRoom = new Room
            {
                Id = 3,
                Description = "C"
            };

            // Create fake RoomRepository. 
            fakeRoomRepository = new Mock<IRepository<Room>>();

            // Implement fake GetAll() method.
            fakeRoomRepository.Setup(x => x.GetAll()).Returns(rooms);

            fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns((int i) => rooms.FirstOrDefault(r => r.Id == i));

            fakeRoomRepository.Setup(x => x.Add(It.IsAny<Room>())).Callback(() =>
            {
                rooms.Add(newRoom);
            });
            // Integers from 1 to 2 (using a range)
            //fakeRoomRepository.Setup(x =>
            //    x.Get(It.IsInRange<int>(1, 2, Moq.Range.Inclusive))).Returns(rooms[1]);
            
            // Any integer:
            //fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(rooms[1]);
            
            // Implement fake Get() method.
            //fakeRoomRepository.Setup(x => x.Get(2)).Returns(rooms[1]);


            // Alternative setup with argument matchers:
            
            // Integers from 1 to 2 (using a predicate)
            // If the fake Get is called with an another argument value than 1 or 2,
            // it returns null, which corresponds to the behavior of the real
            // repository's Get method.
            //fakeRoomRepository.Setup(x => x.Get(It.Is<int>(id => id > 0 && id < 3))).Returns(rooms[1]);

            // Create RoomsController
            controller = new RoomsController(fakeRoomRepository.Object);
        }

        [Fact]
        public void GetAll_ReturnsListWithCorrectNumberOfRooms()
        {
            // Act
            var result = controller.Get() as List<Room>;
            var noOfRooms = result.Count;

            // Assert
            Assert.Equal(2, noOfRooms);
        }

        [Fact]
        public void GetAll_CallsRepoExactlyOnce()
        {
            // Act
            controller.Get();
            
            //Verify
            fakeRoomRepository.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public void GetById_RoomExists_ReturnsIActionResultWithRoom()
        {
            // Act
            var result = controller.Get(2) as ObjectResult;
            var room = result.Value as Room;
            var roomId = room.Id;

            // Assert
            Assert.InRange<int>(roomId, 1, 2);
        }

        [Fact]
        public void GetById_RoomExists_CallsRepoOnce()
        {
            // Arrange
            var id = 1;
            
            // Act
            controller.Get(id);
            
            // Verify
            fakeRoomRepository.Verify(x => x.Get(id), Times.Once);
        }

        [Fact]
        public void GetById_RoomDoesntExist_ReturnsNotFound()
        {
            //Act
            var result = controller.Get(3);
            var notFoundResult = result as NotFoundResult;
            // Assert
            Assert.Equal(404, notFoundResult?.StatusCode);
        }

        [Fact]
        public void Post_WithRoom_ReturnNewRoute()
        {
            // Arrange
            var room = new Room { Id = 1, Description = "C" };
            // Act
            var result = controller.Post(room) as CreatedAtRouteResult;
            // Assert
            Assert.Equal("GetRooms", result?.RouteName);
        }

        [Fact]
        public void Post_WithRoom_AddsRoomToTheRoomList()
        {
            // Arrange
            var room = new Room { Id = 3, Description = "C" };
            // Act
            controller.Post(room);
            // Assert
            Assert.Equal(3, rooms.Count);
        }

        [Fact]
        public void Post_WithNoRoom_ReturnsBadRequest()
        {
            // Act
            var result = controller.Post(null);
            var badRequestResult = result as BadRequestResult;
            
            // Assert
            Assert.Equal(400, badRequestResult?.StatusCode);
        }

        [Fact]
        public void Delete_WhenIdIsLargerThanZero_RemoveIsCalled()
        {
            // Act
            controller.Delete(1);

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(1), Times.Once);
        }

        [Fact]
        public void Delete_WhenIdIsLessThanOne_RemoveIsNotCalled()
        {
            // Act
            controller.Delete(0);

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void Delete_WhenIdIsLargerThanTwo_RemoveThrowsException()
        {
            // Instruct the fake Remove method to throw an InvalidOperationException, if a room id that
            // does not exist in the repository is passed as a parameter. This behavior corresponds to
            // the behavior of the real repoository's Remove method.
            fakeRoomRepository.Setup(x =>
                    x.Remove(It.Is<int>(id => id < 1 || id > 2))).
                    Throws<InvalidOperationException>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => controller.Delete(3));

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()));
        }
    }
}
