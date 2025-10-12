using Business.DTO;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class MenuItemService
    {
        private readonly SqlContext _context;

        public MenuItemService(SqlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItemDTO>> GetAllMenuItemsAsync(CancellationToken cancellationToken = default)
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.IsAvailable)
                .ToListAsync(cancellationToken);

            return menuItems.Select(MapToDto);
        }

        public async Task<IEnumerable<MenuItemDTO>> GetMenuItemsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.CategoryId == categoryId && m.IsAvailable)
                .ToListAsync(cancellationToken);

            return menuItems.Select(MapToDto);
        }

        public async Task<MenuItemDTO?> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            return menuItem != null ? MapToDto(menuItem) : null;
        }

        public async Task<IEnumerable<MenuItemDTO>> GetRecommendedMenuItemsAsync(CancellationToken cancellationToken = default)
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.IsRecommended && m.IsAvailable)
                .ToListAsync(cancellationToken);

            return menuItems.Select(MapToDto);
        }

        public async Task<IEnumerable<MenuItemDTO>> GetVegetarianMenuItemsAsync(CancellationToken cancellationToken = default)
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.IsVegetarian && m.IsAvailable)
                .ToListAsync(cancellationToken);

            return menuItems.Select(MapToDto);
        }


        public async Task<MenuItemDTO> CreateMenuItemAsync(MenuItemDTO menuItemDto)
        {
            var menuItem = new MenuItem
            {
                Name = menuItemDto.Name,
                Description = menuItemDto.Description,
                Price = menuItemDto.Price,
                CategoryId = menuItemDto.CategoryId,
                IsAvailable = menuItemDto.IsAvailable,
                Allergens = menuItemDto.Allergens,
                ImageUrl = menuItemDto.ImageUrl,
                PreparationTime = menuItemDto.PreparationTime,
                IsRecommended = menuItemDto.IsRecommended,
                IsVegetarian = menuItemDto.IsVegetarian,
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            menuItem = await _context.MenuItems
                .Include(m => m.Category)
                .FirstAsync(m => m.Id == menuItem.Id);

            return MapToDto(menuItem);
        }

        public async Task<bool> UpdateMenuItemAsync(int id, MenuItemDTO menuItemDto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return false;

            menuItem.Name = menuItemDto.Name;
            menuItem.Description = menuItemDto.Description;
            menuItem.Price = menuItemDto.Price;
            menuItem.CategoryId = menuItemDto.CategoryId;
            menuItem.IsAvailable = menuItemDto.IsAvailable;
            menuItem.Allergens = menuItemDto.Allergens;
            menuItem.ImageUrl = menuItemDto.ImageUrl;
            menuItem.PreparationTime = menuItemDto.PreparationTime;
            menuItem.IsRecommended = menuItemDto.IsRecommended;
            menuItem.IsVegetarian = menuItemDto.IsVegetarian;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return false;

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAvailabilityAsync(int id, bool isAvailable)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return false;

            menuItem.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        private MenuItemDTO MapToDto(MenuItem menuItem)
        {
            return new MenuItemDTO
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                CategoryId = menuItem.CategoryId,
                CategoryName = menuItem.Category?.Name,
                IsAvailable = menuItem.IsAvailable,
                Allergens = menuItem.Allergens,
                ImageUrl = menuItem.ImageUrl,
                PreparationTime = menuItem.PreparationTime,
                IsRecommended = menuItem.IsRecommended,
                IsVegetarian = menuItem.IsVegetarian,
            };
        }
    }
}
