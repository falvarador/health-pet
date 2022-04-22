using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/pets")]
public class PetController : ControllerBase
{
    private readonly IDbConnection _connection;

    private readonly ILogger<PetController> _logger;

    public PetController(IDbConnection connection, ILogger<PetController> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpDelete("{IDOwner}/{id:int}")]
    public async Task<IActionResult> DeletePet(string IDOwner, int id)
    {
        _logger.LogInformation("Deleting pet");

        var pet = await _connection.QueryFirstOrDefaultAsync<PetDto>(@"
            select PetId, PetTypeId, Name, Age, Breed
            from dbo.Pets
            where PetId = @PetId", new { PetId = id });

        if (pet is null)
        {
            _logger.LogInformation("Pet not found");
            return NotFound();
        }

        await _connection.ExecuteAsync(@"
            delete from dbo.OwnerPets
            where PetId = @PetId
            and ID_Owner = @IDOwner", new { PetId = id, IDOwner });

        await _connection.ExecuteAsync(@"
            delete from dbo.Pets
            where PetId = @PetId", new { PetId = id });

        _logger.LogInformation("Pet deleted");

        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<PetWithDescriptionDto> GetPetById(int id)
    {
        _logger.LogInformation("Getting pet by id");

        var pet = await _connection.QueryFirstOrDefaultAsync<PetWithDescriptionDto>(@"
            select pet.PetId, pet.Name, pet.Age, pet.Breed, 
                petype.PetTypeId, petype.[Description] as PetTypeDescription
            from dbo.Pets pet 
            inner join dbo.PetTypes petype
                on pet.PetTypeId = petype.PetTypeId
            where pet.PetId = @PetId", new { PetId = id });

        _logger.LogInformation("Pet retrieved");

        return pet;
    }

    [HttpPost]
    public async Task<IActionResult> AddPet([FromBody] PetDto petDto)
    {
        _logger.LogInformation("Adding pet");

        var petId = await _connection.ExecuteScalarAsync<int>(@"
            insert into dbo.Pets (Name, Age, Breed, PetTypeId)
            values (@Name, @Age, @Breed, @PetTypeId);
            select cast(scope_identity() as int)", petDto);

        var ownerId = await _connection.QuerySingleOrDefaultAsync<int>(@"
            select OwnerId from dbo.Owners
            where ID = @IDOwner", new { IDOwner = petDto.IDOwner });

        await _connection.ExecuteAsync(@"
            insert into OwnerPets (PetId, OwnerId, ID_Owner)
            values (@PetId, @OwnerId, @ID_Owner);",
                new
                {
                    PetId = petId,
                    OwnerId = ownerId,
                    ID_Owner = petDto.IDOwner
                });

        _logger.LogInformation("Pet added");

        return Created($"api/pets/{petId}",
            new Pet
            {
                PetId = petId,
                Name = petDto.Name,
                Age = petDto.Age,
                Breed = petDto.Breed,
                PetTypeId = petDto.PetTypeId
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePet(int id, [FromBody] PetDto petDto)
    {
        _logger.LogInformation("Updating pet");

        var pet = await _connection.QueryFirstOrDefaultAsync<PetDto>(@"
            select PetId, PetTypeId, Name, Age, Breed
            from dbo.Pets
            where PetId = @PetId", new { PetId = id });

        if (pet is null)
            return await AddPet(petDto);

        await _connection.ExecuteAsync(@"
            update dbo.Pets
            set Name = @Name, Age = @Age, Breed = @Breed, PetTypeId = @PetTypeId
            where PetId = @PetId",
                new
                {
                    PetId = id,
                    petDto.Name,
                    petDto.Age,
                    petDto.Breed,
                    petDto.PetTypeId
                });

        _logger.LogInformation("Pet updated");

        return NoContent();
    }
}
