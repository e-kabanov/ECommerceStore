using ECommerceStore.DTOs;
using ECommerceStore.Models;
using ECommerceStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECommerceStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
           try
            {
                int cartCount = await _cartRepository.AddItem(productId, quantity);
                TempData["Success"] = "Товар добавлен в корзину!";

                return Redirect(Request.Headers["Referer"].ToString() ?? "/Home/Index");
            }

            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            
            try
            {
                var cartcount = await _cartRepository.RemoveItem(cartItemId);
                TempData["Success"] = "Товар удалён из корзины";
                return RedirectToAction("Index");
            }
            
            catch(Exception ex)
            {
                TempData["Error"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("Index");
            } 

        }

        [HttpPost]
        public async Task<IActionResult> RemoveItemCompletely(int cartItemId)
        {
            try
            {
                await _cartRepository.RemoveItemCompletely(cartItemId);
                TempData["Success"] = "Товар полностью удалён из корзины";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = await _cartRepository.DoCheckout(model);

            if (result)
            {
                TempData["Success"] = "Заказ успешно оформлен! Спасибо за покупку!";
                return RedirectToAction("OrderConfirmation");
            }

            else
            {
                TempData["Error"] = "Ошибка при оформлении заказа. Попробуйте позже.";
                return View(model);
            }


        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartItems = await _cartRepository.GetUserCart();
            return View(cartItems);
        }
    }
}
