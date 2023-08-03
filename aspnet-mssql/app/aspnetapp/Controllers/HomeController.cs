using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetapp.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using aspnetapp.Services;

namespace aspnetapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BookStoreDatabaseSettings _myConfiguration;

        private IMongoCollection<Book> _books;
        public HomeController(IConfiguration configuration, IOptions<BookStoreDatabaseSettings> myConfiguration)
        {
            _configuration = configuration;
            _myConfiguration = myConfiguration.Value;
        }
        private MySqlConnection GetConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new MySqlConnection(connectionString);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            var client = new MongoClient(_myConfiguration.ConnectionString);
            var database = client.GetDatabase(_myConfiguration.DatabaseName);
            _books = database.GetCollection<Book>(_myConfiguration.BooksCollectionName);
            string dumb = "";
            foreach(Book book in _books.Find(s => true).ToList<Book>())
            {
                dumb += book.BookName;
                Console.WriteLine(book.BookName);
            }
            ViewData["Message"] = dumb;
            return View();
        }

        public IActionResult Contact()
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM city", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        string cityName = reader.GetString("Name");
                        Console.WriteLine(cityName);
                    }
                }
            }
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
