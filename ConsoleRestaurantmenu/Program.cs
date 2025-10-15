using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using MyLib;

namespace RestaurantMenu
{
    class Program
    {
        static List<Dish> menu = new List<Dish>();
        static List<Dish> cart = new List<Dish>();
        const string AdminPassword = "1234";
        static string orderFileName = "Чек заказа.docx";
        static List<string> categories = new List<string>();

        static void Main(string[] args)
        {
            MenuManager menuManager = new MenuManager();
            CartManager cartManager = new CartManager(menuManager);
            AdminManager adminManager = new AdminManager(menuManager);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== МЕНЮ РЕСТОРАНА ===");
                Console.WriteLine("1. Просмотр меню по категориям");
                Console.WriteLine("2. Поиск блюда");
                Console.WriteLine("3. Корзина");
                Console.WriteLine("4. Режим администратора");
                Console.WriteLine("5. Выход");

                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        menuManager.ShowCategories(cartManager);
                        break;
                    case "2":
                        menuManager.SearchDishes(cartManager);
                        break;
                    case "3":
                        cartManager.ShowCart();
                        break;
                    case "4":
                        adminManager.EnterAdminMode();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        Console.ReadKey();
                        break;
                }
            }
        }


    }
  
}




