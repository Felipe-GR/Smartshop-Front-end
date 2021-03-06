using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Smartshop_FrontEnd.Models;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace Smartshop_FrontEnd.Controllers
{
    public class PaymentController : Controller
    {
        private readonly SmartshopContext _context;
        private string connection = "";

        public PaymentController(SmartshopContext context, IConfiguration configuration)
        {
            _context = context;
            connection = configuration.GetConnectionString("Database");
        }

        public IActionResult Index()
        {
            double price = GetPrice();
            double vat = GetVat(price);

            ViewBag.totalPrice = decimal.Round((decimal)TotalPrice(price, vat), 2, MidpointRounding.AwayFromZero);

            return View();
        }

        public async Task<IActionResult> Bill()
        {
            double price = GetPrice();
            double vat = GetVat(price);

            ViewBag.price = decimal.Round((decimal)price, 2, MidpointRounding.AwayFromZero);
            ViewBag.vat = decimal.Round((decimal)vat, 2, MidpointRounding.AwayFromZero);
            ViewBag.totalPrice = decimal.Round((decimal)TotalPrice(price, vat), 2, MidpointRounding.AwayFromZero);

            return View(await _context.Products.ToListAsync());
        }

        public double GetPrice()
        {
            double price = 0.0;

            string query = "SELECT SUM(ProductPrice) FROM Product";

            using (SqlConnection conn = new SqlConnection(connection))
            {

                conn.Open();

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                price = reader.GetDouble(0);
                            }
                        } catch
                        {
                            price = 0;
                        }

                    }
                }
            }

            return price;
        }

        public double GetVat(double price)
        {
            return (price * 0.19);
        }

        public double TotalPrice(double price, double vatPrice)
        {
            return (price + vatPrice);
        }

        public string GetLastMail()
        {
            string last_shop = "";

            string query = "SELECT TOP 1 * FROM Shopper ORDER BY ID DESC";

            using (SqlConnection conn = new SqlConnection(connection))
            {

                conn.Open();

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            last_shop = reader.GetString(1);
                        }
                    }
                }
            }

            return last_shop;
        }

        public int GetQuantity(string ProductCardId)
        {
            int quantity = 0;

            string query = "SELECT COUNT(*) FROM [dbo].[Product] WHERE CardId = '" + ProductCardId + "'";

            using (SqlConnection conn = new SqlConnection(connection))
            {

                conn.Open();

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            quantity = reader.GetInt32(0);
                        }
                    }
                }
            }

            return quantity;
        }

        public void DropCart()
        {
            string query = "TRUNCATE TABLE Product";

            using (SqlConnection conn = new SqlConnection(connection))
            {

                conn.Open();

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }

                conn.Close();
            }

        }

        public int GetBill()
        {
            int quantity = 0;

            string query = "SELECT COUNT(*) FROM Bill WHERE ProductName = 'Pollo'";

            using (SqlConnection conn = new SqlConnection(connection))
            {

                conn.Open();

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            quantity = reader.GetInt32(0);
                        }
                    }
                }
            }

            return quantity;
        }

        public void SaveBill (string name, double price, int quantity)
        {
            string queryUser = "INSERT INTO Bill (ShopperId) SELECT TOP (1) Id FROM Shopper ORDER BY Id DESC";
            string queryProduct = "UPDATE Bill SET ProductName = '" + name + "', ProductPrice = " + price +", ProductQuantity = " + quantity + "WHERE Id IN (SELECT TOP (1) Id FROM Bill ORDER BY Id DESC)";
            using (SqlConnection conn = new SqlConnection(connection))
            {

                conn.Open();

                using (SqlCommand command = new SqlCommand(queryUser, conn))
                {
                    command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand(queryProduct, conn))
                {
                    command.ExecuteNonQuery();
                }


                conn.Close();
            }

        }

        [HttpPost]
        public IActionResult SendMail()
        {
            var fromAddress = new MailAddress("demosmartshop@gmail.com", "Fast Market Cart");
            var toAddress = new MailAddress(GetLastMail(), "Cliente");
            var copyAddress = new MailAddress("trabajosupc0@gmail.com", "Fast Market Cart");
            const string fromPassword = "1234ABcD*";

            string date = DateTime.UtcNow.ToString("MM-dd-yyyy");
            string subject = "Factura de Venta " + date;

            int productUno = GetQuantity("17430352") != 0 ? GetQuantity("17430352") : 0;
            int productDos = GetQuantity("C7E72C1B") != 0 ? GetQuantity("C7E72C1B") : 0;

            double price = GetPrice();
            double vat = GetVat(price);
            double total = TotalPrice(price, vat);
            double billNo = GetBill();

            string original = @"<head><style>.invoice-box {max-width: 800px;margin: auto;padding: 30px;border: 1px solid #eee;box-shadow: 0 0 10px rgba(0, 0, 0, 0.15);font-size: 16px;line-height: 24px;font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;color: #555;}.invoice-box table {width: 100%;line-height: inherit;text-align: left;}.invoice-box table td {padding: 5px;vertical-align: top;}.invoice-box table tr td:nth-child(2) {text-align: right;}.invoice-box table tr.top table td {padding-bottom: 20px;}.invoice-box table tr.top table td.title {font-size: 45px;line-height: 45px;color: #333;}.invoice-box table tr.information table td {padding-bottom: 40px;}.invoice-box table tr.heading td {background: #eee;border-bottom: 1px solid #ddd;font-weight: bold;}.invoice-box table tr.details td {padding-bottom: 20px;}.invoice-box table tr.item td {border-bottom: 1px solid #eee;}.invoice-box table tr.item.last td {border-bottom: none;}.invoice-box table tr.total td:nth-child(2) {border-top: 2px solid #eee;font-weight: bold;}@media only screen and (max-width: 600px) {.invoice-box table tr.top table td {width: 100%;display: block;text-align: center;}.invoice-box table tr.information table td {width: 100%;display: block;text-align: center;}}.rtl {direction: rtl;font-family: Tahoma, 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;}.rtl table {text-align: right;}.rtl table tr td:nth-child(2) {text-align: left;}</style></head><body><div class=""invoice-box""><table cellpadding=""0"" cellspacing=""0""><tr class=""top""><td colspan=""2""><table><tr><td class=""title""><img src=""https://i.ibb.co/zJXM63r/e23bd6f9-a7c3-4e3a-a423-172632ec4efe.jpg"" style=""width: 100%; max-width: 300px"" /></td><td>Invoice #: INVNO<br />Created: Today</td></tr></table></td></tr><tr class=""information""><td colspan=""2""><table><tr><td>Fast Market Cart<br />Carrera 9 #45A-44<br />Bogotá, Colombia</td><td></td><td>mdaza@tesis.com</td></tr></table></td></tr><tr class=""heading""><td>Producto</td><td>Cantidad</td><td>Precio</td></tr><tr class=""item""><td>Pollo</td><td>P1</td><td>PrecioUno</td></tr><tr class=""item""><td>Queso</td><td>P2</td><td>PrecioDos</td></tr><tr class=""total""><td></td><td></td><td>Subtotal: $ST</td></tr><tr class=""total""><td></td><td></td><td>IVA: $IVATAX</td></tr><tr class=""total""><td></td><td></td><td>total: $PTOTAL</td></tr></table></div></body>";
            string body = original.Replace("P1", productUno.ToString()).Replace("P2", productDos.ToString()).Replace("ST", price.ToString()).Replace("IVATAX", vat.ToString()).Replace("PTOTAL", total.ToString()).Replace("INVNO", (billNo + 1).ToString()).Replace("Today", DateTime.Now.ToString()).Replace("mdaza@tesis.com", "Cliente: " + GetLastMail()).Replace("PrecioUno", (15000 * productUno).ToString()).Replace("PrecioDos", (2000 * productDos).ToString());

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                message.IsBodyHtml = true;
                message.Bcc.Add(copyAddress);
                //smtp.Send(message);
            }

            SaveBill("Queso", 15000, productUno);
            SaveBill("Pollo", 2000, productDos);
            DropCart();

            return RedirectToAction("Create", "Shoppers");
        }

    }
}
