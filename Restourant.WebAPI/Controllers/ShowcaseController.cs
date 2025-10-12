using Business.DTO;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Restourant.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowcaseController : ControllerBase
    {
        private readonly ILogger<ShowcaseController> _logger;
        private readonly MenuItemService _menuItemService;
        private readonly CategoryService _categoryService;

        public ShowcaseController(
            ILogger<ShowcaseController> logger,
            MenuItemService menuItemService,
            CategoryService categoryService)
        {
            _logger = logger;
            _menuItemService = menuItemService;
            _categoryService = categoryService;
        }

        [HttpGet("menu")]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetMenu()
        {
            var menuItems = await _menuItemService.GetAllMenuItemsAsync();
            return Ok(menuItems);
        }

        [HttpGet("menu/category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetMenuByCategory(int categoryId)
        {
            var menuItems = await _menuItemService.GetMenuItemsByCategoryAsync(categoryId);
            return Ok(menuItems);
        }

        [HttpGet("menu/{id}")]
        public async Task<ActionResult<MenuItemDTO>> GetMenuItem(int id)
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);

            if (menuItem == null)
                return NotFound();

            return Ok(menuItem);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("menu/recommended")]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetRecommendedItems()
        {
            var recommendedItems = await _menuItemService.GetRecommendedMenuItemsAsync();
            return Ok(recommendedItems);
        }

        [HttpGet("menu/vegetarian")]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetVegetarianItems()
        {
            var vegetarianItems = await _menuItemService.GetVegetarianMenuItemsAsync();
            return Ok(vegetarianItems);
        }

        [HttpPost("menu")]
        public async Task<ActionResult<MenuItemDTO>> CreateMenuItem([FromBody] MenuItemDTO menuItemDto)
        {
            _logger.LogInformation("Creating menu item: {Name}, Category: {CategoryId}, Price: {Price}",
                menuItemDto.Name, menuItemDto.CategoryId, menuItemDto.Price);

            var createdItem = await _menuItemService.CreateMenuItemAsync(menuItemDto);

            _logger.LogInformation("Menu item created with ID: {Id}", createdItem.Id);

            return CreatedAtAction(nameof(GetMenuItem), new { id = createdItem.Id }, createdItem);
        }

        [HttpPut("menu/{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItemDTO menuItemDto)
        {
            _logger.LogInformation("Updating menu item ID: {Id}", id);

            var result = await _menuItemService.UpdateMenuItemAsync(id, menuItemDto);

            if (!result)
                return NotFound();

            _logger.LogInformation("Menu item ID: {Id} updated", id);
            return NoContent();
        }

        [HttpDelete("menu/{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            _logger.LogWarning("Deleting menu item ID: {Id}", id);

            var result = await _menuItemService.DeleteMenuItemAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("menu/{id}/availability")]
        public async Task<IActionResult> SetAvailability(int id, [FromBody] bool isAvailable)
        {
            _logger.LogInformation("Setting availability for menu item ID: {Id} to {IsAvailable}", id, isAvailable);

            var result = await _menuItemService.SetAvailabilityAsync(id, isAvailable);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(
    [FromBody] CategoryDTO categoryDTO,
    CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating category: {Name}", categoryDTO.Name);

            var createdCategory = await _categoryService.CreateCategoryAsync(categoryDTO, cancellationToken);

            _logger.LogInformation("Category created with ID: {Id}", createdCategory.Id);

            return CreatedAtAction(nameof(GetCategories), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] CategoryDTO categoryDTO,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating category ID: {Id}", id);

            var result = await _categoryService.UpdateCategoryAsync(id, categoryDTO, cancellationToken);

            if (!result)
                return NotFound();

            _logger.LogInformation("Category ID: {Id} updated", id);
            return NoContent();
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Deleting category ID: {Id}", id);

            var result = await _categoryService.DeleteCategoryAsync(id, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }

    }
}
