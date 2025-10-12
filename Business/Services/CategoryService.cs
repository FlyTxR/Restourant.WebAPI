using Business.DTO;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class CategoryService
    {
        private readonly SqlContext _context;

        public CategoryService(SqlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(cancellationToken);

            return categories.Select(MapToDTO);
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            return category != null ? MapToDTO(category) : null;
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDTO, CancellationToken cancellationToken = default)
        {
            var category = new Category
            {
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                DisplayOrder = categoryDTO.DisplayOrder,
                IconUrl = categoryDTO.IconUrl
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDTO(category);
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryDTO categoryDTO, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (category == null) return false;

            category.Name = categoryDTO.Name;
            category.Description = categoryDTO.Description;
            category.DisplayOrder = categoryDTO.DisplayOrder;
            category.IconUrl = categoryDTO.IconUrl;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private CategoryDTO MapToDTO(Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                IconUrl = category.IconUrl
            };
        }
    }
}
