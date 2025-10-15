using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class MenuManager
    {
        public List<Dish> Menu { get; private set; } = new List<Dish>();
        public List<string> Categories { get; private set; } = new List<string>();

        public MenuManager()
        {
            InitializeMenu();
        }
        public void InitializeMenu()
        {
            Categories.AddRange(new[] { "Салаты", "Супы", "Горячие блюда", "Десерты", "Напитки" });

            Menu.Add(new Dish("Салат Цезарь", "Салаты", 350, "Курица, листья салата, соус Цезарь"));
            Menu.Add(new Dish("Борщ", "Супы", 250, "Свекла, мясо, картофель"));
            Menu.Add(new Dish("Стейк", "Горячие блюда", 650, "Говядина, специи"));
            Menu.Add(new Dish("Чизкейк", "Десерты", 350, "Творожный сыр, ягоды"));
            Menu.Add(new Dish("Кола", "Напитки", 150, "Газированный напиток"));
            Menu.Add(new Dish("Греческий", "Салаты", 400, "Помидоры, сыр фета, свежие огурцы, сладкий перец, черные маслины"));
            Menu.Add(new Dish("Солянка", "Супы", 230, "Ветчина, копченая колбаса, соленые огурцы, маслины, оливки"));
            Menu.Add(new Dish("Лосось на гриле", "Горячие блюда", 800, "Стейк лосося, пряности, тушеные овощи, зелень"));
            Menu.Add(new Dish("Брауни", "Десерты", 600, "Шоколад, карамельный соус, мороженное ванильное"));
            Menu.Add(new Dish("Сок Клубничный", "Напитки", 150, "Клубника, сахар"));
        }
        public void ShowCategories(CartManager cart)
        {
            Console.Clear();
            Console.WriteLine("=== КАТЕГОРИИ ===");

            for (int i = 0; i < Categories.Count; i++)
                Console.WriteLine($"{i + 1}. {Categories[i]}");

            Console.Write("\nВыберите категорию (0 - назад): ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == 0) return;
                if (choice > 0 && choice <= Categories.Count)
                    ShowDishesByCategory(Categories[choice - 1], cart);
            }
        }

        public void ShowDishesByCategory(string category, CartManager cart)
        {
            var dishes = Menu.Where(d => d.Category == category).ToList();
            Console.Clear();
            Console.WriteLine($"=== {category.ToUpper()} ===");

            for (int i = 0; i < dishes.Count; i++)
                Console.WriteLine($"{i + 1}. {dishes[i].Name} - {dishes[i].Price} руб. ({dishes[i].Description})");

            Console.Write("\nВыберите блюдо для добавления в корзину (0 - назад): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= dishes.Count)
            {
                Console.Write("Количество: ");
                if (int.TryParse(Console.ReadLine(), out int qty) && qty > 0)
                {
                    cart.AddToCart(dishes[choice - 1], qty);
                }
            }
        }

        // ПОИСК
        public void SearchDishes(CartManager cart)
        {
            Console.Clear();
            Console.WriteLine("=== ПОИСК БЛЮДА ===");
            Console.Write("Введите название: ");
            var search = Console.ReadLine()?.ToLower();

            var found = Menu.Where(d => d.Name.ToLower().Contains(search)).ToList();

            if (!found.Any())
            {
                Console.WriteLine("Блюда не найдены.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < found.Count; i++)
                Console.WriteLine($"{i + 1}. {found[i].Name} ({found[i].Category}) - {found[i].Price} руб.");

            Console.Write("\nВыберите блюдо для добавления в корзину (0 - назад): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= found.Count)
            {
                Console.Write("Количество: ");
                if (int.TryParse(Console.ReadLine(), out int qty) && qty > 0)
                    cart.AddToCart(found[choice - 1], qty);
            }
        }

        // МЕТОДЫ АДМИНИСТРАТОРА
        public void AddCategory(string name)
        {
            if (!Categories.Any(c => c.Equals(name, StringComparison.OrdinalIgnoreCase)))
                Categories.Add(name);
        }

        public void AddDish(Dish dish)
        {
            Menu.Add(dish);
        }

        public void RemoveDish(string name)
        {
            var dish = Menu.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (dish != null) Menu.Remove(dish);
        }
    }
}
