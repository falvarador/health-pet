using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/pet-types")]
public class PetTypeController : ControllerBase
{
    private readonly IDbConnection _connection;

    private readonly ILogger<PetTypeController> _logger;

    public PetTypeController(IDbConnection connection, ILogger<PetTypeController> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePetType(int id)
    {
        _logger.LogInformation("Deleting pet type");

        var petType = await _connection.QueryFirstOrDefaultAsync<PetType>(@"
            select PetTypeId, Description
            from dbo.PetTypes
            where PetTypeId = @PetTypeId", new { PetTypeId = id });

        if (petType is null)
        {
            _logger.LogInformation("Pet type not found");
            return NotFound();
        }

        await _connection.ExecuteAsync(@"
            delete from dbo.PetTypes
            where PetTypeId = @PetTypeId", new { PetTypeId = id });

        _logger.LogInformation("Pet type deleted");

        return NoContent();
    }

    [HttpGet]
    public async Task<IEnumerable<PetType>> GetPetTypes()
    {
        _logger.LogInformation("Getting pet types");

        var petTypes = await _connection.QueryAsync<PetType>(@"
            select PetTypeId, Description
            from dbo.PetTypes
            order by PetTypeId");

        _logger.LogInformation("Pet types retrieved");

        return petTypes;
    }

    [HttpGet("{id:int}")]
    public async Task<PetType> GetPetType(int id)
    {
        _logger.LogInformation("Getting pet type");

        var petType = await _connection.QueryFirstOrDefaultAsync<PetType>(@"
            select PetTypeId, Description
            from dbo.PetTypes
            where PetTypeId = @PetTypeId", new { PetTypeId = id });

        _logger.LogInformation("Pet type retrieved");

        return petType;
    }

    [HttpPost]
    public async Task<IActionResult> AddPetType([FromBody] PetTypeDto petTypeDto)
    {
        _logger.LogInformation("Adding pet type");

        var petTypeId = await _connection.ExecuteScalarAsync<int>(@"
            insert into dbo.PetTypes (Description)
            values (@Description);
            select cast(scope_identity() as int)", petTypeDto);

        _logger.LogInformation("Pet type added");

        return Created($"api/pet-types/{petTypeId}",
            new PetType
            {
                PetTypeId = petTypeId,
                Description = petTypeDto.Description
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePetType(int id, [FromBody] PetTypeDto petTypeDto)
    {
        _logger.LogInformation("Updating pet type");

        var petType = await _connection.QueryFirstOrDefaultAsync<PetType>(@"
            select PetTypeId, Description
            from dbo.PetTypes
            where PetTypeId = @PetTypeId", new { PetTypeId = id });

        if (petType is null)
            return await AddPetType(petTypeDto);

        await _connection.ExecuteAsync(@"
            update dbo.PetTypes
            set Description = @Description
            where PetTypeId = @PetTypeId",
                new { PetTypeId = id, petTypeDto.Description });

        _logger.LogInformation("Pet type updated");

        return NoContent();
    }
}
