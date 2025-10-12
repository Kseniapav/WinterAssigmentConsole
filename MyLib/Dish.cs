using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class Dish
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }

        public Dish(string name, string category, int price, string description)
        {
            Name = name;
            Category = category;
            Price = price;
            Description = description;
        }
    }
}
