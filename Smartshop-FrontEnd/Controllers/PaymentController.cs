using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Smartshop_FrontEnd.Models;
using System;
using System.Threading.Tasks;

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
    }
}
