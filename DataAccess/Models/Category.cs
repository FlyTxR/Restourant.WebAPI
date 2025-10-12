using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public string? IconUrl { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
    }

}
