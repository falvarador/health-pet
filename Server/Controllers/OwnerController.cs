using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/owners")]
public class OwnerController : ControllerBase
{
    private readonly IDbConnection _connection;

    private readonly ILogger<OwnerController> _logger;

    public OwnerController(IDbConnection connection, ILogger<OwnerController> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOwner(int id)
    {
        _logger.LogInformation("Deleting owner");

        var owner = await _connection.QueryFirstOrDefaultAsync<Owner>(@"
            select OwnerId, Name, Address, Phone
            from dbo.Owners
            where OwnerId = @OwnerId", new { OwnerId = id });

        if (owner is null)
        {
            _logger.LogInformation("Owner not found");
            return NotFound();
        }

        await _connection.ExecuteAsync(@"
            delete from dbo.Owners
            where OwnerId = @OwnerId", new { OwnerId = id });

        _logger.LogInformation("Owner deleted");

        return NoContent();
    }

    [HttpGet("{ID}")]
    public async Task<IEnumerable<Owner>> GetOwners(string ID)
    {
        _logger.LogInformation("Getting owners");

        var owners = await _connection.QueryAsync<Owner>(@"
            select OwnerId, Name, LastName, ID, Phone, Email
            from dbo.Owners
            where ID = @ID
            order by OwnerId", new { ID });

        _logger.LogInformation("Owners retrieved");

        return owners;
    }

    [HttpGet("{IDOwner}/pets")]
    public async Task<IEnumerable<PetWithDescriptionDto>> GetPets(string IDOwner)
    {
        _logger.LogInformation("Getting pets");

        var pets = await _connection.QueryAsync<PetWithDescriptionDto>(@"
            select pet.PetId, pet.Name, pet.Age, pet.Breed, petype.[Description] as PetTypeDescription, ID_Owner as IDOwner
            from dbo.Pets pet 
            inner join dbo.PetTypes petype
                on pet.PetTypeId = petype.PetTypeId
            inner join dbo.OwnerPets ownpet
                on ownpet.PetId = pet.PetId
            where ID_Owner = @IDOwner
            order by pet.PetId", new { IDOwner });

        _logger.LogInformation("Pets retrieved");

        return pets;
    }


    [HttpGet("{IDOwner}/appointments")]
    public async Task<IEnumerable<AppointmentWithDescriptionDto>> GetAppointments(string IDOwner)
    {
        _logger.LogInformation("Getting appointments");

        var ownerId = await _connection.QuerySingleOrDefaultAsync<int>(@"
            select OwnerId from dbo.Owners
            where ID = @IDOwner", new { IDOwner });

        var appointments = await _connection.QueryAsync<AppointmentWithDescriptionDto>(@"
            select AppointmentId, app.OwnerId, CONCAT(own.Name, ' ', own.LastName) as OwnerDescription, 
                app.CategoryId, cat.[Description] as CategoryDescription, Hour, Date, State
            from dbo.Appointments app
            inner join dbo.Owners own
                on own.OwnerId = app.OwnerId
            inner join dbo.Categories cat
                on cat.CategoryId = app.CategoryId
            where app.OwnerId = @ownerId
            order by app.Hour, app.Date", new { ownerId });

        _logger.LogInformation("Appointments retrieved");

        return appointments;
    }

    [HttpPost]
    public async Task<IActionResult> AddOwner([FromBody] OwnerDto ownerDto)
    {
        _logger.LogInformation("Adding owner");

        var ownerId = await _connection.ExecuteScalarAsync<int>(@"
            insert into dbo.Owners (Name, LastName, ID, Phone, Email)
            values (@Name, @LastName, @ID, @Phone, @Email)", ownerDto);

        _logger.LogInformation("Owner added");

        return Created($"api/owners/{ownerId}",
           new Owner
           {
               OwnerId = ownerId,
               Name = ownerDto.Name,
               LastName = ownerDto.LastName,
               ID = ownerDto.ID,
               Phone = ownerDto.Phone,
               Email = ownerDto.Email
           });
    }

    [HttpPut("{ID}")]
    public async Task<IActionResult> UpdateOwner(int ID, [FromBody] OwnerDto ownerDto)
    {
        _logger.LogInformation("Updating owner");

        var owner = await _connection.QueryFirstOrDefaultAsync<Owner>(@"
            select OwnerId, Name, LastName, ID, Phone, Email
            from dbo.Owners
            where ID = @ID", new { ID });

        if (owner is null)
            return await AddOwner(ownerDto);

        await _connection.ExecuteAsync(@"
            update dbo.Owners
            set Name = @Name, LastName = @LastName, ID = @ID, Phone = @Phone, Email = @Email
            where OwnerId = @OwnerId",
                new
                {
                    OwnerId = owner.OwnerId,
                    ownerDto.Name,
                    ownerDto.LastName,
                    ownerDto.ID,
                    ownerDto.Phone,
                    ownerDto.Email
                });

        _logger.LogInformation("Owner updated");

        return NoContent();
    }
}
