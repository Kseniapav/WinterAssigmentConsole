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
            InitializeMenu();

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
                        ShowCategories();
                        break;
                    case "2":
                        SearchDishes();
                        break;
                    case "3":
                        ShowCart();
                        break;
                    case "4":
                        AdminMode();
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

        static void AdminMode()
        {
            Console.Clear();
            Console.WriteLine("=== РЕЖИМ АДМИНИСТРАТОРА ===");
            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();

            if (password != AdminPassword)
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
                Console.WriteLine("2. Редактировать категорию");
                Console.WriteLine("3. Удалить категорию");
                Console.WriteLine("4. Добавить блюдо");
                Console.WriteLine("5. Редактировать блюдо");
                Console.WriteLine("6. Удалить блюдо");
                Console.WriteLine("0. Вернуться в главное меню");

                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddCategory();
                        break;
                    case "2":
                        EditCategory();
                        break;
                    case "3":
                        RemoveCategory();
                        break;
                    case "4":
                        AddDish();
                        break;
                    case "5":
                        EditDish();
                        break;
                    case "6":
                        RemoveDish();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void AddCategory()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ДОБАВЛЕНИЕ КАТЕГОРИИ ===");
                Console.WriteLine("0. Назад");
                Console.Write("Введите название новой категории: ");
                var category = Console.ReadLine();

                if (category == "0") return;

                if (!string.IsNullOrWhiteSpace(category))
                {
                    // Проверка на наличие цифр в названии категории
                    if (category.Any(char.IsDigit))
                    {
                        Console.WriteLine("Название категории не может содержать цифры!");
                        Console.ReadKey();
                        continue;
                    }

                    if (!categories.Any(c => c.Equals(category, StringComparison.OrdinalIgnoreCase)))
                    {
                        categories.Add(category);
                        Console.WriteLine($"Категория '{category}' успешно добавлена!");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"Категория '{category}' уже существует!");
                    }
                }
                else
                {
                    Console.WriteLine("Название категории не может быть пустым!");
                }
                Console.ReadKey();
            }
        }

        static void EditCategory()
        {
            Console.Clear();
            Console.WriteLine("=== РЕДАКТИРОВАНИЕ КАТЕГОРИИ ===");

            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных категорий для редактирования.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Список категорий:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            Console.Write("\nВыберите категорию для редактирования (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out int categoryChoice))
            {
                if (categoryChoice == 0) return;

                if (categoryChoice > 0 && categoryChoice <= categories.Count)
                {
                    string oldCategoryName = categories[categoryChoice - 1];
                    Console.Write($"Введите новое название для категории '{oldCategoryName}': ");
                    string newCategoryName = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(newCategoryName))
                    {
                        // Проверка на наличие цифр в названии категории
                        if (newCategoryName.Any(char.IsDigit))
                        {
                            Console.WriteLine("Название категории не может содержать цифры!");
                            Console.ReadKey();
                            return;
                        }

                        if (!categories.Any(c => c.Equals(newCategoryName, StringComparison.OrdinalIgnoreCase)))
                        {
                            // Обновляем название категории в списке категорий
                            categories[categoryChoice - 1] = newCategoryName;

                            // Обновляем категорию у всех связанных блюд
                            foreach (var dish in menu.Where(d => d.Category == oldCategoryName))
                            {
                                dish.Category = newCategoryName;
                            }

                            Console.WriteLine($"Категория '{oldCategoryName}' успешно изменена на '{newCategoryName}'!");
                        }
                        else
                        {
                            Console.WriteLine($"Категория '{newCategoryName}' уже существует!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Название категории не может быть пустым!");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный номер категории.");
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод. Введите число.");
            }
            Console.ReadKey();
        }

        static void RemoveCategory()
        {
            Console.Clear();
            Console.WriteLine("=== УДАЛЕНИЕ КАТЕГОРИИ ===");

            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных категорий для удаления.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Список категорий:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            Console.Write("\nВыберите категорию для удаления (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out int categoryChoice))
            {
                if (categoryChoice == 0) return;

                if (categoryChoice > 0 && categoryChoice <= categories.Count)
                {
                    string categoryToRemove = categories[categoryChoice - 1];

                    // Проверяем, есть ли блюда в этой категории
                    bool hasDishes = menu.Any(d => d.Category == categoryToRemove);

                    if (hasDishes)
                    {
                        Console.WriteLine($"В категории '{categoryToRemove}' есть блюда. Удалить категорию и все связанные блюда? (да/нет)");
                        var confirm = Console.ReadLine().ToLower();

                        if (confirm != "да")
                        {
                            Console.WriteLine("Удаление отменено.");
                            Console.ReadKey();
                            return;
                        }

                        // Удаляем все блюда этой категории
                        menu.RemoveAll(d => d.Category == categoryToRemove);
                    }

                    // Удаляем саму категорию
                    categories.RemoveAt(categoryChoice - 1);
                    Console.WriteLine($"Категория '{categoryToRemove}' и все связанные блюда удалены!");
                }
                else
                {
                    Console.WriteLine("Неверный номер категории.");
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод. Введите число.");
            }
            Console.ReadKey();
        }

        static void AddDish()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ДОБАВЛЕНИЕ БЛЮДА ===");
                Console.WriteLine("0. Назад");

                if (categories.Count == 0)
                {
                    Console.WriteLine("Сначала добавьте хотя бы одну категорию!");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Доступные категории:");
                for (int i = 0; i < categories.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {categories[i]}");
                }

                Console.Write("Выберите категорию (0 - отмена): ");
                if (int.TryParse(Console.ReadLine(), out int categoryIndex))
                {
                    if (categoryIndex == 0) return;

                    if (categoryIndex > 0 && categoryIndex <= categories.Count)
                    {
                        string category = categories[categoryIndex - 1];

                        Console.Write("Введите название блюда: ");
                        var name = Console.ReadLine();
                        if (name == "0") return;

                        Console.Write("Введите цену: ");
                        var priceInput = Console.ReadLine();
                        if (priceInput == "0") return;

                        if (int.TryParse(priceInput, out int price) && price > 0)
                        {
                            Console.Write("Введите описание: ");
                            var description = Console.ReadLine();
                            if (description == "0") return;

                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                menu.Add(new Dish(name, category, price, description));
                                Console.WriteLine($"Блюдо '{name}' успешно добавлено в категорию '{category}'!");
                                Console.ReadKey();
                                return;
                            }
                            else
                            {
                                Console.WriteLine("Название блюда не может быть пустым!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Цена должна быть положительным числом!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор категории!");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Введите число.");
                }
                Console.ReadKey();
            }
        }

        static void EditDish()
        {
            Console.Clear();
            Console.WriteLine("=== РЕДАКТИРОВАНИЕ БЛЮДА ===");

            if (menu.Count == 0)
            {
                Console.WriteLine("Меню пусто. Нет блюд для редактирования.");
                Console.ReadKey();
                return;
            }

            // Группируем блюда по категориям для удобного отображения
            var dishesByCategory = menu.GroupBy(d => d.Category);

            foreach (var categoryGroup in dishesByCategory)
            {
                Console.WriteLine($"\nКатегория: {categoryGroup.Key}");
                int index = 1;
                foreach (var dish in categoryGroup)
                {
                    Console.WriteLine($"  {index++}. {dish.Name} - {dish.Price} руб. ({dish.Description})");
                }
            }

            Console.Write("\nВведите название блюда для редактирования (0 - отмена): ");
            string dishName = Console.ReadLine();
            if (dishName == "0") return;

            var dishToEdit = menu.FirstOrDefault(d => d.Name.Equals(dishName, StringComparison.OrdinalIgnoreCase));

            if (dishToEdit != null)
            {
                Console.WriteLine($"\nРедактирование блюда: {dishToEdit.Name}");
                Console.WriteLine("1. Изменить название");
                Console.WriteLine("2. Изменить категорию");
                Console.WriteLine("3. Изменить цену");
                Console.WriteLine("4. Изменить описание");
                Console.WriteLine("0. Назад");

                Console.Write("Выберите что изменить: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Введите новое название: ");
                        string newName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newName))
                        {
                            dishToEdit.Name = newName;
                            Console.WriteLine("Название блюда успешно изменено!");
                        }
                        else
                        {
                            Console.WriteLine("Название не может быть пустым!");
                        }
                        break;
                    case "2":
                        Console.WriteLine("Доступные категории:");
                        for (int i = 0; i < categories.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {categories[i]}");
                        }
                        Console.Write("Выберите новую категорию: ");
                        if (int.TryParse(Console.ReadLine(), out int newCategoryIndex) && 
                            newCategoryIndex > 0 && newCategoryIndex <= categories.Count)
                        {
                            dishToEdit.Category = categories[newCategoryIndex - 1];
                            Console.WriteLine("Категория блюда успешно изменена!");
                        }
                        else
                        {
                            Console.WriteLine("Неверный выбор категории!");
                        }
                        break;
                    case "3":
                        Console.Write("Введите новую цену: ");
                        if (int.TryParse(Console.ReadLine(), out int newPrice) && newPrice > 0)
                        {
                            dishToEdit.Price = newPrice;
                            Console.WriteLine("Цена блюда успешно изменена!");
                        }
                        else
                        {
                            Console.WriteLine("Цена должна быть положительным числом!");
                        }
                        break;
                    case "4":
                        Console.Write("Введите новое описание: ");
                        string newDescription = Console.ReadLine();
                        dishToEdit.Description = newDescription;
                        Console.WriteLine("Описание блюда успешно изменено!");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine($"Блюдо с названием '{dishName}' не найдено.");
                Console.ReadKey();
            }
        }

        static void RemoveDish()
        {
            Console.Clear();
            Console.WriteLine("=== УДАЛЕНИЕ БЛЮДА ===");

            if (menu.Count == 0)
            {
                Console.WriteLine("Меню пусто. Нет блюд для удаления.");
                Console.ReadKey();
                return;
            }

            // Группируем блюда по категориям для удобного отображения
            var dishesByCategory = menu.GroupBy(d => d.Category);

            foreach (var categoryGroup in dishesByCategory)
            {
                Console.WriteLine($"\nКатегория: {categoryGroup.Key}");
                int index = 1;
                foreach (var dish in categoryGroup)
                {
                    Console.WriteLine($"  {index++}. {dish.Name} - {dish.Price} руб.");
                }
            }

            Console.Write("\nВведите название блюда для удаления (0 - отмена): ");
            string dishName = Console.ReadLine();
            if (dishName == "0") return;

            var dishToRemove = menu.FirstOrDefault(d => d.Name.Equals(dishName, StringComparison.OrdinalIgnoreCase));

            if (dishToRemove != null)
            {
                menu.Remove(dishToRemove);
                Console.WriteLine($"Блюдо '{dishToRemove.Name}' удалено из меню!");
            }
            else
            {
                Console.WriteLine($"Блюдо с названием '{dishName}' не найдено.");
            }
            Console.ReadKey();
        }

        static void InitializeMenu()
        {
            // Инициализация категорий
            categories.Add("Салаты");
            categories.Add("Супы");
            categories.Add("Горячие блюда");
            categories.Add("Десерты");
            categories.Add("Напитки");

            // Инициализация меню
            menu.Add(new Dish("Салат Цезарь", "Салаты", 350, "Курица, листья салата, сухарики, соус Цезарь"));
            menu.Add(new Dish("Греческий салат", "Салаты", 300, "Помидоры, огурцы, оливки, сыр фета"));
            menu.Add(new Dish("Борщ", "Супы", 250, "Свекла, мясо, картофель, капуста"));
            menu.Add(new Dish("Солянка", "Супы", 280, "Различные виды мяса, огурцы, каперсы"));
            menu.Add(new Dish("Стейк", "Горячие блюда", 650, "Говядина, специи"));
            menu.Add(new Dish("Лосось на гриле", "Горячие блюда", 550, "Лосось, лимон, зелень"));
            menu.Add(new Dish("Тирамису", "Десерты", 300, "Печенье савоярди, кофе, сыр маскарпоне"));
            menu.Add(new Dish("Чизкейк", "Десерты", 350, "Творожный сыр, печенье, ягоды"));
            menu.Add(new Dish("Кола", "Напитки", 150, "Газированный напиток"));
            menu.Add(new Dish("Апельсиновый сок", "Напитки", 200, "Свежевыжатый апельсиновый сок"));
        }

        static void ShowCategories()
        {
            Console.Clear();
            Console.WriteLine("=== КАТЕГОРИИ ===");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            Console.Write("\nВыберите категорию (0 - назад): ");
            if (int.TryParse(Console.ReadLine(), out int categoryChoice))
            {
                if (categoryChoice == 0) return;

                if (categoryChoice > 0 && categoryChoice <= categories.Count)
                {
                    var selectedCategory = categories[categoryChoice - 1];
                    ShowDishesByCategory(selectedCategory);
                }
                else
                {
                    Console.WriteLine("Неверный номер категории. Попробуйте снова.");
                    Console.ReadKey();
                    ShowCategories();
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод. Введите число.");
                Console.ReadKey();
                ShowCategories();
            }
        }

        static void ShowDishesByCategory(string category)
        {
            Console.Clear();
            var dishesInCategory = menu.Where(d => d.Category == category).ToList();

            Console.WriteLine($"=== {category.ToUpper()} ===");
            if (dishesInCategory.Count == 0)
            {
                Console.WriteLine("В этой категории пока нет блюд.");
            }
            else
            {
                for (int i = 0; i < dishesInCategory.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {dishesInCategory[i].Name} - {dishesInCategory[i].Price} руб.");
                    Console.WriteLine($"   {dishesInCategory[i].Description}");
                    Console.WriteLine();
                }
            }

            Console.Write("\nВыберите блюдо для добавления в корзину (0 - назад): ");
            if (int.TryParse(Console.ReadLine(), out int dishChoice))
            {
                if (dishChoice == 0) return;

                if (dishChoice > 0 && dishChoice <= dishesInCategory.Count)
                {
                    Console.Write("Количество: ");
                    if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                    {
                        var selectedDish = dishesInCategory[dishChoice - 1];
                        for (int i = 0; i < quantity; i++)
                        {
                            cart.Add(selectedDish);
                        }
                        Console.WriteLine($"{quantity} x {selectedDish.Name} добавлено в корзину!");
                    }
                    else
                    {
                        Console.WriteLine("Неверное количество! Должно быть положительное число.");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный номер блюда. Попробуйте снова.");
                }
                Console.ReadKey();
                ShowDishesByCategory(category);
            }
            else
            {
                Console.WriteLine("Неверный ввод. Введите число.");
                Console.ReadKey();
                ShowDishesByCategory(category);
            }
        }

        static void SearchDishes()
        {
            Console.Clear();
            Console.WriteLine("=== ПОИСК БЛЮДА ===");
            Console.Write("Введите название блюда: ");
            var searchTerm = Console.ReadLine().ToLower();

            var foundDishes = menu.Where(d => d.Name.ToLower().Contains(searchTerm)).ToList();

            if (foundDishes.Any())
            {
                Console.WriteLine("\nРезультаты поиска:");
                for (int i = 0; i < foundDishes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {foundDishes[i].Name} ({foundDishes[i].Category}) - {foundDishes[i].Price} руб.");
                }

                Console.Write("\nВыберите блюдо для добавления в корзину (0 - назад): ");
                if (int.TryParse(Console.ReadLine(), out int dishChoice))
                {
                    if (dishChoice == 0) return;

                    if (dishChoice > 0 && dishChoice <= foundDishes.Count)
                    {
                        Console.Write("Количество: ");
                        if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                        {
                            var selectedDish = foundDishes[dishChoice - 1];
                            for (int i = 0; i < quantity; i++)
                            {
                                cart.Add(selectedDish);
                            }
                            Console.WriteLine($"{quantity} x {selectedDish.Name} добавлено в корзину!");
                        }
                        else
                        {
                            Console.WriteLine("Неверное количество! Должно быть положительное число.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер блюда.");
                    }
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Введите число.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Блюда не найдены.");
                Console.ReadKey();
            }
        }

        static void ShowCart()
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

                var groupedCart = cart.GroupBy(d => d.Name)
                                     .Select(g => new CartItem { Dish = g.First(), Count = g.Count() })
                                     .ToList();

                for (int i = 0; i < groupedCart.Count; i++)
                {
                    var item = groupedCart[i];
                    Console.WriteLine($"{i + 1}. {item.Dish.Name} - {item.Dish.Price} руб. x {item.Count} = {item.Dish.Price * item.Count} руб.");
                }

                Console.WriteLine($"\nОбщая стоимость: {cart.Sum(d => d.Price)} руб.");

                Console.WriteLine("\n1. Добавить блюдо");
                Console.WriteLine("2. Удалить блюдо");
                Console.WriteLine("3. Очистить корзину");
                Console.WriteLine("4. Оформить заказ");
                Console.WriteLine("0. Назад");

                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddToCartFromCart();
                        break;
                    case "2":
                        RemoveFromCart(groupedCart);
                        break;
                    case "3":
                        cart.Clear();
                        Console.WriteLine("Корзина очищена.");
                        Console.ReadKey();
                        break;
                    case "4":
                        Checkout();
                        return;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void AddToCartFromCart()
        {
            Console.Clear();
            Console.WriteLine("=== ДОБАВЛЕНИЕ БЛЮДА В КОРЗИНУ ===");

            Console.WriteLine("1. Добавить из меню");
            Console.WriteLine("2. Добавить из корзины");
            Console.WriteLine("0. Назад");

            Console.Write("Выберите действие: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowCategories();
                    break;
                case "2":
                    AddExistingDishFromCart();
                    break;
                case "0":
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    Console.ReadKey();
                    break;
            }
        }

        static void AddExistingDishFromCart()
        {
            var groupedCart = cart.GroupBy(d => d.Name)
                                 .Select(g => new CartItem { Dish = g.First(), Count = g.Count() })
                                 .ToList();

            Console.WriteLine("Список блюд в корзине:");
            for (int i = 0; i < groupedCart.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {groupedCart[i].Dish.Name} (в корзине: {groupedCart[i].Count})");
            }

            Console.Write("Выберите блюдо для добавления (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out int dishChoice))
            {
                if (dishChoice == 0) return;

                if (dishChoice > 0 && dishChoice <= groupedCart.Count)
                {
                    Console.Write("Количество для добавления: ");
                    if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                    {
                        var selectedDish = groupedCart[dishChoice - 1].Dish;
                        for (int i = 0; i < quantity; i++)
                        {
                            cart.Add(selectedDish);
                        }
                        Console.WriteLine($"{quantity} x {selectedDish.Name} добавлено в корзину!");
                    }
                    else
                    {
                        Console.WriteLine("Неверное количество! Должно быть положительное число.");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный номер блюда.");
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод. Введите число.");
            }
            Console.ReadKey();
        }

        static void RemoveFromCart(List<CartItem> groupedCart)
        {
            Console.Write("Выберите блюдо для удаления (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out int itemChoice))
            {
                if (itemChoice == 0) return;

                if (itemChoice > 0 && itemChoice <= groupedCart.Count)
                {
                    var itemToRemove = groupedCart[itemChoice - 1];

                    Console.Write($"Сколько {itemToRemove.Dish.Name} удалить (всего {itemToRemove.Count}, 0 - отмена)? ");
                    if (int.TryParse(Console.ReadLine(), out int removeCount))
                    {
                        if (removeCount == 0) return;

                        if (removeCount > 0 && removeCount <= itemToRemove.Count)
                        {
                            for (int i = 0; i < removeCount; i++)
                            {
                                var index = cart.FindIndex(d => d.Name == itemToRemove.Dish.Name);
                                if (index != -1) cart.RemoveAt(index);
                            }
                            Console.WriteLine($"Удалено {removeCount} x {itemToRemove.Dish.Name}");
                        }
                        else
                        {
                            Console.WriteLine("Неверное количество! Должно быть от 1 до " + itemToRemove.Count);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный ввод. Введите число.");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный номер блюда.");
                }
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Неверный ввод. Введите число.");
                Console.ReadKey();
            }
        }

        static void Checkout()
        {
            Console.Clear();
            Console.WriteLine("=== ОФОРМЛЕНИЕ ЗАКАЗА ===");

            var groupedCart = cart.GroupBy(d => d.Name)
                                 .Select(g => new CartItem { Dish = g.First(), Count = g.Count() })
                                 .ToList();

            foreach (var item in groupedCart)
            {
                Console.WriteLine($"{item.Dish.Name} - {item.Dish.Price} руб. x {item.Count} = {item.Dish.Price * item.Count} руб.");
            }

            Console.WriteLine($"\nОбщая стоимость: {cart.Sum(d => d.Price)} руб.");

            while (true)
            {
                Console.Write("\nОформить заказ? (да/нет): ");
                var confirm = Console.ReadLine().ToLower();

                if (confirm == "да")
                {
                    if (SaveOrderToWordFile())
                    {
                        cart.Clear();
                        Console.WriteLine("Заказ оформлен и сохранен в файл 'Чек заказа.docx'");
                        Console.WriteLine("Открываю файл с чеком...");
                        Console.ReadKey();

                        try
                        {
                            Process.Start(new ProcessStartInfo(orderFileName) { UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Не удалось открыть файл: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при сохранении заказа!");
                    }
                    Console.ReadKey();
                    return;
                }
                else if (confirm == "нет")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Пожалуйста, введите 'да' или 'нет'.");
                }
            }
        }

        static bool SaveOrderToWordFile()
        {
            try
            {
                // Создаем или открываем документ Word
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(orderFileName, WordprocessingDocumentType.Document))
                {
                    // Добавляем главную часть документа
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Добавляем заголовок
                    Paragraph titleParagraph = body.AppendChild(new Paragraph());
                    Run titleRun = titleParagraph.AppendChild(new Run());
                    titleRun.AppendChild(new Text("=== ЧЕК ЗАКАЗА ==="));
                    titleRun.RunProperties = new RunProperties(new Bold());

                    // Добавляем дату
                    Paragraph dateParagraph = body.AppendChild(new Paragraph());
                    dateParagraph.AppendChild(new Run(new Text($"Дата: {DateTime.Now}")));

                    // Добавляем пустую строку
                    body.AppendChild(new Paragraph());

                    // Добавляем позиции заказа
                    var groupedCart = cart.GroupBy(d => d.Name)
                                         .Select(g => new CartItem { Dish = g.First(), Count = g.Count() })
                                         .ToList();

                    foreach (var item in groupedCart)
                    {
                        Paragraph itemParagraph = body.AppendChild(new Paragraph());
                        itemParagraph.AppendChild(new Run(new Text($"{item.Dish.Name} - {item.Dish.Price} руб. x {item.Count} = {item.Dish.Price * item.Count} руб.")));
                    }

                    // Добавляем итоговую сумму
                    body.AppendChild(new Paragraph());
                    Paragraph totalParagraph = body.AppendChild(new Paragraph());
                    Run totalRun = totalParagraph.AppendChild(new Run());
                    totalRun.AppendChild(new Text($"ИТОГО: {cart.Sum(d => d.Price)} руб."));
                    totalRun.RunProperties = new RunProperties(new Bold());

                    // Добавляем благодарность
                    body.AppendChild(new Paragraph());
                    Paragraph thanksParagraph = body.AppendChild(new Paragraph());
                    thanksParagraph.AppendChild(new Run(new Text("Спасибо за заказ!")));

                    // Сохраняем документ (в папке bin)
                    mainPart.Document.Save();
                }
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = orderFileName,
                    UseShellExecute = true
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении чека: {ex.Message}");
                return false;
            }
        }


    }
  
}




