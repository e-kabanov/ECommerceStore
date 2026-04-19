using System.Diagnostics;
using ECommerceStore.DTOs;
using ECommerceStore.Models;
using ECommerceStore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepository;

        public HomeController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            IEnumerable<Product> products = await _homeRepository.GetProducts(searchTerm);

            var model = new ProductDisplayModel
            {
                Products = products,
                SearchTerm = searchTerm
            };

            return View(model);
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
