using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly IDbConnection _connection;

    private readonly ILogger<CategoryController> _logger;

    public CategoryController(IDbConnection connection, ILogger<CategoryController> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        _logger.LogInformation("Deleting category");

        var category = await _connection.QueryFirstOrDefaultAsync<CategoryDto>(@"
            select CategoryId, Description
            from dbo.Categories
            where CategoryId = @CategoryId", new { CategoryId = id });

        if (category is null)
        {
            _logger.LogInformation("Category not found");
            return NotFound();
        }

        await _connection.ExecuteAsync(@"
            delete from dbo.Categories
            where CategoryId = @CategoryId", new { CategoryId = id });

        _logger.LogInformation("Category deleted");

        return NoContent();
    }

    [HttpGet]
    public async Task<IEnumerable<Category>> GetCategories()
    {
        _logger.LogInformation("Getting categories");

        var categories = await _connection.QueryAsync<Category>(@"
            select CategoryId, Description
            from dbo.Categories
            order by CategoryId");

        _logger.LogInformation("Categories retrieved");

        return categories;
    }

    [HttpGet("{id:int}")]
    public async Task<Category> GetCategory(int id)
    {
        _logger.LogInformation("Getting category");

        var category = await _connection.QueryFirstOrDefaultAsync<Category>(@"
            select CategoryId, Description
            from dbo.Categories
            where CategoryId = @CategoryId", new { CategoryId = id });

        _logger.LogInformation("Category retrieved");

        return category;
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryDto categoryDto)
    {
        _logger.LogInformation("Adding category");

        var categoryId = await _connection.ExecuteScalarAsync<int>(@"
            insert into dbo.Categories ([Description])
            values (@Description);
            select cast(scope_identity() as int)", categoryDto);

        _logger.LogInformation("Category added");

        return Created($"api/categories/{categoryId}",
            new Category
            {
                CategoryId = categoryId,
                Description = categoryDto.Description
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryDto categoryDto)
    {
        _logger.LogInformation("Updating category");

        var category = await _connection.QueryFirstOrDefaultAsync<CategoryDto>(@"
            select CategoryId, Description
            from dbo.Categories
            where CategoryId = @CategoryId", new { CategoryId = id });

        if (category is null)
            return await AddCategory(categoryDto);

        await _connection.ExecuteAsync(@"
            update dbo.Categories
            set [Description] = @Description
            where CategoryId = @CategoryId",
                new { CategoryId = id, categoryDto.Description });

        _logger.LogInformation("Category updated");

        return NoContent();
    }
}
