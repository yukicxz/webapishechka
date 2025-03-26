using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
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
        [HttpGet("{id}")]
        public string GetProductID(Guid id)
        {
            _logger.LogInformation($"Поиск продукта по id:{id}");
            var product = _productService.Search(id);

            if (product == null)
            {
                _logger.LogWarning($"Продукт не найден");
                return "Продукт не найден";
            }
            _logger.LogInformation($"Продукт найден");
            return "Найден";
        }
        [HttpPost]
        public IActionResult AddProduct([FromBody]Product product)
        {
            if (product == null)
            {
                _logger.LogWarning($"Продукт не добавлен");
                return BadRequest("Продукт не добавлен");
            }
            _logger.LogInformation($"Добавление продукта {product.Name}");
            var AddedProduct = _productService.Add(product);
            if(AddedProduct == null)
            {
                _logger.LogWarning($"Продукт не добавлен");
                return BadRequest("Продукт не добавлен");
            }
            _logger.LogInformation($"Добавлен продукт: {product.Name}, c id: {product.Id}");
            return Ok($"Добавлен продукт: {product.Name}, c id: {product.Id}");
        }
        [HttpPut]
        public IActionResult EditProduct([FromBody] Product product)
        {
            _logger.LogInformation($"Изменение продукта {product.Name}");
            var EditedProduct = _productService.Edit(product);
            if (EditedProduct == null)
            {
                _logger.LogWarning($"Продукт не изменен");
                return BadRequest("Продукт не изменен");
            }
            _logger.LogInformation($"Изменен продукт: {product.Name},Описание:{product.Definition},цена: {product.Price}");
            return Ok($"Изменен продукт: {product.Name},Описание:{product.Definition},цена: {product.Price}");
        }
        [HttpDelete("{id}")]
        public IActionResult RemoveProduct(Guid id)
        {
            _logger.LogInformation($"Удаление продукта по id:{id}");
            var RemovedProduct = _productService.Remove(id);
            if (RemovedProduct == null)
            {
                _logger.LogWarning($"Продукт не удален");
                return BadRequest("Продукт не удален");
            }
            _logger.LogInformation($"Удален продукт: {RemovedProduct.Name}");
            return Ok("Удален продукт");
        }
    }
}
