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
                        while (reader.Read())
                        {
                            price = reader.GetDouble(0);
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

        public void SendMail()
        {
            MailAddress to = new MailAddress("trabajosupc0@gmail.com");
            MailAddress from = new MailAddress("luisguerron@hotmail.com");

            MailMessage message = new MailMessage(from, to);
            message.Subject = "Good morning, Elizabeth";
            message.Body = "Elizabeth, Long time no talk. Would you be up for lunch in Soho on Monday? I'm paying.;";

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("trabajosupc0@gmail.com", "Trabajos123*"),
                EnableSsl = true
            };

            try
            {
                client.Send(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
