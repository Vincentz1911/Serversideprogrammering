using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serversideprogrammering.Data;
using Serversideprogrammering.Models;
using BC = BCrypt.Net.BCrypt;

namespace Serversideprogrammering.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDBContext _context;
        private readonly IConfiguration _config;
        private readonly IDataProtector _protector;

        public HomeController(ILogger<HomeController> logger, 
            MyDBContext context, 
            IConfiguration config,
            IDataProtectionProvider provider)
        {
            _logger = logger;
            _context = context;
            _config = config;
            _protector = provider.CreateProtector(_config["CryptoKey"]);
        }

        private void showViewBagMessage()
        {
            string FlashMessage = HttpContext.Session.GetString("FlashMessage");
            if (FlashMessage != null)
            {
                ViewBag.FlashMessage = FlashMessage;
                HttpContext.Session.Remove("FlashMessage");
            }
        }

        async public Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Redirect("/Home/Login");

            ViewBag.User = await _context.User
                .Include(u => u.TodoItems)
                .FirstOrDefaultAsync(u => u.Id == userId);

            foreach (TodoItem item in ViewBag.User.TodoItems)
            {
                try
                {
                    item.Title = _protector.Unprotect(item.Title);
                    item.Description = _protector.Unprotect(item.Description);
                }
                catch (Exception e) { Console.WriteLine(e); };
            }

            showViewBagMessage();
            return View();
        }

        [HttpGet]
        public IActionResult CreateTodo() 
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Redirect("/Home/Login");
            showViewBagMessage();
            return View(); 
        }

        [HttpPost]
        public IActionResult CreateTodo(TodoItem todoItem)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Redirect("/Home/Login");

            todoItem.UserId = (int)userId;
            todoItem.Added = DateTime.Now;
            todoItem.Title = _protector.Protect(todoItem.Title);
            todoItem.Description = _protector.Protect(todoItem.Description);

            _context.TodoItem.Add(todoItem);
            _context.SaveChanges();

            ViewBag.Message = "Todo Item created";
            return Redirect("/");
        }

        [HttpGet]
        public IActionResult Register() {
            showViewBagMessage();
            return View(); 
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            user.Password = BC.HashPassword(user.Password);
            user.Email = _protector.Protect(user.Email);
            _context.User.Add(user);
            _context.SaveChanges();

            return Redirect("/");
        }

        [HttpGet]
        public IActionResult Login() {
            showViewBagMessage();
            return View(); 
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == null || password == null)
            {
                ViewBag.Message = "Please fill out both forms";
                return View();
            }

            string email = _protector.Protect(username);
            User user = _context.User.FirstOrDefault(u => u.Username == username || u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "No such user exists (or wrong password)";
                return View();
            }
            else
            {
                if (!BC.Verify(password, user.Password))
                {
                    ViewBag.Message = "Wrong password (or unknown user)";
                    return View();
                }
                else
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    Console.WriteLine($"User {user.Username} logged in");
                    return Redirect("/");
                }
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Home/Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
