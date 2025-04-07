using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        //Получить продукт по Id
        [HttpGet("{id}")]
        public string GetProductById(Guid id)
        {
            _logger.LogInformation($"Поиск продукта с Id: {id}.");
            var product = _productService.Search(id);

            if (product == null)
            {
                _logger.LogWarning($"Продукт с ID {id} не найден.");
                return "Не найден";
            }

            return "Найден";
        }

        //Создать новый продукт
        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Продукт не может быть null.");
            }

            var createdProduct = _productService.Add(product);
            if (createdProduct == null)
            {
                _logger.LogWarning("Не удалось добавить товар");
                return BadRequest("Не удалось добавить товар");
            }

            _logger.LogInformation($"Созданный продукт: {product.Name}");
            return Ok("Продукт создан");
        }


        //Обновить существующий продукт
        [HttpPut]
        public string EditProduct(Product product)
        {
            _logger.LogInformation($"Изменение товара: {product.Id}");
            if (_productService.Edit(product) != null)
            {
                _logger.LogWarning($"Обновлен продукт: {product.Name},Описание:{product.Definition},цена: {product.Price}");
                return $"Обновлен продукт: {product.Name},Описание:{product.Definition},цена: {product.Price}";
            }
            return "Продукт не найден";
        }

        //Удалить продукт
        [HttpDelete("{id}")]
        public string DeleteProduct(Guid id)
        {
            _logger.LogInformation($"Удаление продукта {id}.");
            var result = _productService.Remove(id);

            if (result != null)
            {
                _logger.LogWarning($"Не удалось удалить продукт с ID: {id}.");
                return "Не удалось удалить";
            }

            return "Удалено";
        }
    }
}
