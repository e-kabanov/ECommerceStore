using ECommerceStore.DTOs;
using ECommerceStore.Models;
using ECommerceStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ECommerceStore.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IUserOrderRepository _repository;
        private readonly IFileService _fileService;
        private readonly IProductRepository _productRepository;
        public AdminController(IUserOrderRepository repository, IFileService fileService, IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
            _fileService = fileService;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _repository.AllOrders();

            return View(orders);
        }

        [HttpGet("update-status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {

            var order = await _repository.GetOrderById(orderId);
            var statuses = await _repository.GetOrderStatuses();

            var model = new UpdateOrderStatusModel
            {
                OrderId = order.OrderId,
                OrderStatusId = order.StatusId,
                OrderStatusList = statuses.Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.StatusName,
                    Selected = s.StatusId == order.StatusId
                }).ToList()
            };

            return View(model);

        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isUpdated = await _repository.UpdateOrderStatus(model);

            if (isUpdated)
            {
                TempData["Success"] = "Статус заказа обновлён";
            }

            else
            {
                TempData["Error"] = "Заказ не найден";
            }

            return RedirectToAction("AllOrders");

        }


        [HttpGet("products")]
        public async Task<IActionResult> Products()
        {
            var products = await _productRepository.GetAllProducts();
            return View(products);
        }

        [HttpGet("products/create")]
        public IActionResult CreateProduct()
        {
            return View(new ProductViewModel());
        }

        [HttpPost("products/create")]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                
                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(model.ImageFile, allowedExtensions);

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Stock = model.Stock,
                    Photo = imageName,
                    CreatedAt = DateTime.Now
                };

                await _productRepository.AddProduct(product);
                TempData["SuccessMessage"] = "Товар успешно добавлен";
                return RedirectToAction(nameof(Products));

            }

            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
             }
        }

        [HttpGet("products/edit/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = $"Товар с ID {id} не найден";
                return RedirectToAction(nameof(Products));
            }

            var model = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Photo = product.Photo
            };

            return View(model);
        }


        [HttpPost("products/edit/{id}")]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string oldImage = model.Photo ?? "";
                string newImage = oldImage;

                if (model.ImageFile != null)
                {
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png", ".webp"];
                    newImage = await _fileService.SaveFile(model.ImageFile, allowedExtensions);
                }

                var product = new Product
                {
                    ProductId = model.ProductId,
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Stock = model.Stock,
                    Photo = newImage
                };

                var updatedProduct = await _productRepository.UpdateProduct(product);
                if (updatedProduct == null)
                {
                    TempData["ErrorMessage"] = "Товар не найден";
                    return RedirectToAction(nameof(Products));
                }

                if (model.ImageFile != null && !string.IsNullOrEmpty(oldImage)) { _fileService.DeleteFile(oldImage); }

                TempData["SuccessMessage"] = "Товар успешно обновлён";
                return RedirectToAction(nameof(Products));
            }

            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при обновлении товара";
                return View(model);
            }

        }

        [HttpPost("products/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetProductById(id);

            if (product != null)
            {
                await _productRepository.DeleteProduct(product);

                if (!string.IsNullOrEmpty(product.Photo))
                {
                    _fileService.DeleteFile(product.Photo);
                }

                TempData["SuccessMessage"] = "Товар удалён";
            }

            else
            {
                TempData["ErrorMessage"] = $"Товар с ID {id} не найден";
            }

            return RedirectToAction(nameof(Products));
        }






    }
}
