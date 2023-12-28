using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Moq;
using Xunit;

public class CarsControllerTests
{

    // Test for the Create method in the CarsController
    [Fact]
    public void Create_AddsNewCar()
    {
        // Mock the database
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Car>>();

        // Setup the database to return the mock collection
        mockDatabase.Setup(db => db.GetCollection<Car>("Cars", It.IsAny<MongoCollectionSettings>()))
                    .Returns(mockCollection.Object);

        // Create the controller
        var carsController = new CarsController(mockDatabase.Object);

        // Car to add
        var newCar = new Car { Id = "3", Make = "Ford", Model = "Mustang", Year = 2022, Price = 40000 };

        // Setup the collection to return the new car when InsertOne is called
        mockCollection.Setup(c => c.InsertOne(It.IsAny<Car>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                      .Callback<Car, InsertOneOptions, CancellationToken>((car, options, token) =>
                      {
                          // Verify that the car has been added 
                          Assert.NotNull(car);
                      });

        // Call the Create method to add the new car
        var result = carsController.Create(newCar);

        // Assert that the result is a CreatedAtActionResult and the car returned is the same as the one added
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var car = Assert.IsType<Car>(createdAtActionResult.Value);
        Assert.Equal(newCar.Id, car.Id);
        Assert.Equal(newCar.Make, car.Make);
        Assert.Equal(newCar.Model, car.Model);
        Assert.Equal(newCar.Year, car.Year);
        Assert.Equal(newCar.Price, car.Price);
    }

    // Test for the Delete method in the CarsController
    [Fact]
    public void Delete_WithNonExistingId_ReturnsNotFound()
    {
        // Mock the database
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Car>>();

        // Setup the database to return the mock collection
        mockDatabase.Setup(db => db.GetCollection<Car>("Cars", It.IsAny<MongoCollectionSettings>()))
                    .Returns(mockCollection.Object);

        // Create the controller
        var carsController = new CarsController(mockDatabase.Object);

        // Id of the car to delete
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // Setup the collection to return acknowledgment for deletion when DeleteOne is called
        mockCollection.Setup(c => c.DeleteOne(It.IsAny<FilterDefinition<Car>>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<Car>, CancellationToken>((filter, token) =>
            {
                // Perform necessary checks on 'filter'
                var expectedFilter = Builders<Car>.Filter.Eq(c => c.Id, nonExistingId);

                // Compare the filters to ensure they are the same
                Assert.Equal(expectedFilter.Render(BsonSerializer.SerializerRegistry.GetSerializer<Car>(), BsonSerializer.SerializerRegistry), 
                filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<Car>(), BsonSerializer.SerializerRegistry));

            })
            .Returns(new DeleteResult.Acknowledged(0)); // Simulate deletion

        // Call the Delete method to delete the car
        var result = carsController.Delete(nonExistingId);

        // Assert that the result is NotFoundResult
        Assert.IsType<NotFoundResult>(result);
    }

}
