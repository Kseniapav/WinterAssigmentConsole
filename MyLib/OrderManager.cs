using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using DocumentFormat.OpenXml;

namespace MyLib
{
    public class OrderManager
    {
        private List<Dish> cart;
        private string orderFileName = "Чек заказа.docx";

        public OrderManager(List<Dish> cart)
        {
            this.cart = cart;
        }

        public bool SaveOrderToWordFile()
        {
            try
            {
                using (var doc = WordprocessingDocument.Create(orderFileName, WordprocessingDocumentType.Document))
                {
                    var main = doc.AddMainDocumentPart();
                    main.Document = new Document(new Body());
                    Body body = main.Document.Body;

                    body.Append(new Paragraph(new Run(new Text("=== ЧЕК ЗАКАЗА ==="))));
                    body.Append(new Paragraph(new Run(new Text($"Дата: {DateTime.Now}"))));
                    body.Append(new Paragraph());

                    var grouped = cart.GroupBy(d => d.Name)
                                      .Select(g => new CartItem { Dish = g.First(), Count = g.Count() });

                    foreach (var item in grouped)
                    {
                        body.Append(new Paragraph(new Run(new Text(
                            $"{item.Dish.Name} - {item.Dish.Price} x {item.Count} = {item.Dish.Price * item.Count} руб."))));
                    }

                    body.Append(new Paragraph());
                    body.Append(new Paragraph(new Run(new Text($"ИТОГО: {cart.Sum(d => d.Price)} руб."))));
                    body.Append(new Paragraph(new Run(new Text("Спасибо за заказ!"))));
                    main.Document.Save();
                }

                Process.Start(new ProcessStartInfo(orderFileName) { UseShellExecute = true });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
        }
    }
}
