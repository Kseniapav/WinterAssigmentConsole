using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class AdminManager
    {
        private const string AdminPassword = "1234";
        private MenuManager menuManager;

        public AdminManager(MenuManager manager)
        {
            menuManager = manager;
        }

        public void EnterAdminMode()
        {
            Console.Clear();
            Console.Write("Введите пароль администратора: ");
            var pass = Console.ReadLine();

            if (pass != AdminPassword)
            {
                Console.WriteLine("Неверный пароль!");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== РЕЖИМ АДМИНИСТРАТОРА ===");
                Console.WriteLine("1. Добавить категорию");
                Console.WriteLine("2. Добавить блюдо");
                Console.WriteLine("3. Удалить блюдо");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Введите название категории: ");
                        menuManager.AddCategory(Console.ReadLine());
                        break;
                    case "2":
                        Console.Write("Название блюда: ");
                        var name = Console.ReadLine();
                        Console.Write("Категория: ");
                        var cat = Console.ReadLine();
                        Console.Write("Цена: ");
                        int price = int.Parse(Console.ReadLine());
                        Console.Write("Описание: ");
                        var desc = Console.ReadLine();
                        menuManager.AddDish(new Dish(name, cat, price, desc));
                        break;
                    case "3":
                        Console.Write("Введите название блюда для удаления: ");
                        menuManager.RemoveDish(Console.ReadLine());
                        break;
                    case "0":
                        return;
                }
            }
        }
    }
}
