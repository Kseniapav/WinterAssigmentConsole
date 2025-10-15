using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class CartManager
    {
        private List<Dish> cart = new List<Dish>();
        private MenuManager menuManager;

        public CartManager(MenuManager menuManager)
        {
            this.menuManager = menuManager;
        }

        public void AddToCart(Dish dish, int quantity)
        {
            for (int i = 0; i < quantity; i++)
                cart.Add(dish);

            Console.WriteLine($"{quantity} x {dish.Name} добавлено в корзину!");
            Console.ReadKey();
        }

        public void ShowCart()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== КОРЗИНА ===");

                if (cart.Count == 0)
                {
                    Console.WriteLine("Корзина пуста.");
                    Console.ReadKey();
                    return;
                }

                var grouped = cart.GroupBy(d => d.Name)
                                  .Select(g => new CartItem { Dish = g.First(), Count = g.Count() })
                                  .ToList();

                foreach (var item in grouped)
                    Console.WriteLine($"{item.Dish.Name} - {item.Dish.Price} x {item.Count} = {item.Dish.Price * item.Count} руб.");

                Console.WriteLine($"\nИтого: {cart.Sum(d => d.Price)} руб.");
                Console.WriteLine("\n1. Удалить блюдо\n2. Оформить заказ\n0. Назад");
                Console.Write("Ваш выбор: ");

                var choice = Console.ReadLine();
                if (choice == "0") return;
                if (choice == "1") RemoveItem(grouped);
                if (choice == "2") Checkout();
            }
        }

        private void RemoveItem(List<CartItem> grouped)
        {
            Console.Write("Введите название блюда для удаления: ");
            var name = Console.ReadLine();
            var item = grouped.FirstOrDefault(i => i.Dish.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                cart.RemoveAll(d => d.Name == item.Dish.Name);
                Console.WriteLine("Блюдо удалено.");
            }
            else Console.WriteLine("Такого блюда нет.");
            Console.ReadKey();
        }

        private void Checkout()
        {
            var order = new OrderManager(cart);
            order.SaveOrderToWordFile();
            cart.Clear();
        }
    }
}
