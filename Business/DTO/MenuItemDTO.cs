using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class MenuItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsAvailable { get; set; }
        public string? Allergens { get; set; }
        public string? ImageUrl { get; set; }
        public int PreparationTime { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsRecommended{ get; set; }

    }

}
