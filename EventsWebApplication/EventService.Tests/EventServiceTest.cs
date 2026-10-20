using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;
using WebApp = EventsWebApplication;

namespace EventService.Tests
{
    public class EventServiceTest
    {
        private IEventService _eventService;

        public static IEnumerable<Event> TestData()
        {
            return
            [
                new Event { Id = Guid.Parse("077c6978-9d5f-4e34-926a-15b003327115"), Title = "Event1", Description = string.Empty, StartAt = DateTime.Now, EndAt = DateTime.Now.AddDays(2) },
                new Event { Title = "Event2", Description = string.Empty, StartAt = DateTime.Now.AddDays(1), EndAt = DateTime.Now.AddDays(3) },
                new Event { Id = Guid.Parse("177c6978-9d5f-4e34-926a-15b003327115"), Title = "Event3", Description = string.Empty, StartAt = DateTime.Now.AddDays(2), EndAt = DateTime.Now.AddDays(4) },
                new Event { Title = "Event4", Description = string.Empty, StartAt = DateTime.Now.AddDays(3), EndAt = DateTime.Now.AddDays(5) },
            ];
        }

        public EventServiceTest()
        {
            _eventService = new WebApp.Services.EventService();
        }

        [Fact]
        [Trait("Category", "Success")]
        public void Create_Event_Success()
        {
            //Arrange
            var newEvent = new Event
            {
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act
            _eventService.Add(newEvent);

            //Assert
            Assert.Equal("Event1", _eventService.GetAll(null, null, null).Items?.FirstOrDefault()?.Title);
        }

        [Fact]
        [Trait("Category", "Success")]
        public void Get_AllEvents_Success()
        {
            //Arrange
            var allEventsCount = 1;

            //Act
            _eventService.Add(
                new Event
                {
                    Title = "Event1",
                    Description = string.Empty,
                    StartAt = DateTime.Now,
                    EndAt = DateTime.Now.AddDays(3)
                });
            var resultCount = _eventService.GetAll(null, null, null).TotalItems;

            //Assert
            Assert.Equal(allEventsCount, resultCount);
        }

        [Fact]
        [Trait("Category", "Success")]
        public void Get_EventById_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var eventNew = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act
            _eventService.Add(eventNew);
            var eventData = _eventService.GetEvent(eventId);

            //Assert
            Assert.Equal(eventId, eventData?.Id);
        }

        [Fact]
        [Trait("Category", "Success")]
        public void Update_Event_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var eventTest = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };
            var newTitle = "Event11";

            //Act
            _eventService.Add(eventTest);
            
            eventTest.Title = newTitle;
            _eventService.Update(eventId, eventTest);

            var eventData = _eventService.GetEvent(eventId);

            //Assert
            Assert.Equal(newTitle, eventData?.Title);
        }

        [Fact]
        [Trait("Category", "Success")]
        public void Delete_Event_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var eventTest = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act
            _eventService.Add(eventTest);
            _eventService.Delete(eventId);

            //Assert
            Assert.Null(_eventService.GetEvent(eventId));
        }

        [Fact]
        [Trait("Category", "Success")]
        public void Filter_ByTitle_Success()
        {
            //Arrange
            var event1 = new Event
            {
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };
            var event2 = new Event
            {
                Title = "Event2",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act
            _eventService.Add(event1);
            _eventService.Add(event2);
            var result = _eventService.GetAll("Event1", null, null).Items;

            //Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        [Trait("Category", "Success")]
        public void Filter_ByDates_Success()
        {
            //Arrange
            var event1StartAt = DateTime.Now;
            var event2EndAt = DateTime.Now.AddDays(3);
            var event1 = new Event
            {
                Title = "Event1",
                Description = string.Empty,
                StartAt = event1StartAt,
                EndAt = DateTime.Now.AddDays(2)
            };
            var event2 = new Event
            {
                Title = "Event2",
                Description = string.Empty,
                StartAt = DateTime.Now.AddDays(1),
                EndAt = event2EndAt
            };

            //Act
            _eventService.Add(event1);
            _eventService.Add(event2);
            var result1 = _eventService.GetAll(null, event1StartAt, null).Items;
            var result2 = _eventService.GetAll(null, null, event2EndAt).Items;

            //Assert
            Assert.NotEmpty(result1);
            Assert.NotEmpty(result2);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [Trait("Category", "Success")]
        public void Get_PagedEvents_Success(int pageNumber, int pageSize)
        {
            //Arrange

            //Act
            foreach (var item in TestData())
            {
                _eventService.Add(item);
            }
            var result = _eventService.GetAll(null, null, null, pageNumber, pageSize).Items;

            //Assert
            Assert.True(result.Count() == pageSize);
        }

        [Fact]
        [Trait("Category", "Unsuccess")]
        public void Get_NotExistEvent_Unsuccess()
        {
            //Arrange
            var id = Guid.NewGuid();

            //Act
            foreach (var item in TestData())
            {
                _eventService.Add(item);
            }
            var result = _eventService.GetEvent(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        [Trait("Category", "Unsuccess")]
        public void Update_Event_Unsuccess()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var eventTest = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };
            var newTitle = "Event11";

            //Act
            _eventService.Add(eventTest);

            eventTest.Title = newTitle;

            //Assert
            Assert.Throws<Exception>(() => _eventService.Update(Guid.NewGuid(), eventTest));
        }

        [Fact]
        [Trait("Category", "Unsuccess")]
        public void Create_Event_Unsuccess()
        {
            //Arrange
            var date = DateTime.Now;
            var newEvent = new Event
            {
                Title = "Event1",
                Description = string.Empty,
                StartAt = date,
                EndAt = date
            };

            //Act & Assert
            Assert.Throws<Exception>(() => _eventService.Add(newEvent));
        }

        [Fact]
        [Trait("Category", "Unsuccess")]
        public void Update_EventWithInncorrectData_Unsuccess()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var eventAdd = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };
            var eventUpdate = new Event
            {
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(-3)
            };

            //Act
            _eventService.Add(eventAdd);

            //Assert
            Assert.Throws<Exception>(() => _eventService.Update(eventId, eventUpdate));
        }
    }
}
