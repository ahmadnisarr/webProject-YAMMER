using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AboutUS()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult features()
        {
            return View();
        }
        public IActionResult solution()
        {
            return View();
        }
    }
}
