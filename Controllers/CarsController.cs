using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class CarsController : ControllerBase
{
    // mongodb variable
    private readonly IMongoCollection<Car> _cars;

    // constructor
    public CarsController(IMongoDatabase database)
    {
        _cars = database.GetCollection<Car>("Cars");
    }

    // GET: api/Cars
    [HttpGet]
    public ActionResult<IEnumerable<Car>> Get()
    {
        var cars = _cars.Find(c => true).ToList();
        return Ok(cars);
    }

    // GET: api/Cars/:id
    [HttpGet("{id}")]
    public ActionResult<Car> GetById(string id)
    {
        var car = _cars.Find(c => c.Id == id).FirstOrDefault();
        if (car == null)
            return NotFound();

        return Ok(car);
    }

    // POST: api/Cars
    [HttpPost]
    public ActionResult<Car> Create([FromBody] Car newCar)
    {
        _cars.InsertOne(newCar);

        return CreatedAtAction(nameof(GetById), new { id = newCar.Id }, newCar);
    }

    // PUT: api/Cars/:id
    [HttpPut("{id}")]
    public ActionResult<Car> Update(string id, [FromBody] Car updatedCar)
    {
        var filter = Builders<Car>.Filter.Eq(c => c.Id, id);
        var update = Builders<Car>.Update
            .Set(c => c.Make, updatedCar.Make)
            .Set(c => c.Model, updatedCar.Model)
            .Set(c => c.Year, updatedCar.Year);

        var result = _cars.UpdateOne(filter, update);

        if (result.ModifiedCount == 0)
            return NotFound();

        return Ok(updatedCar);
    }

    // DELETE: api/Cars/:id
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var result = _cars.DeleteOne(c => c.Id == id);
        if (result.DeletedCount == 0)
            return NotFound();

        return NoContent();
    }
}

