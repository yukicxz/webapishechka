using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Controllers;
using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MagazineTests.Stub
{
    public class StubProductService : IProductService
    {
        public Product ProductToReturn { get; set; }
        public Product Add(Product product) => ProductToReturn;
        public Product Edit(Product product) => ProductToReturn;
        public Product Remove(Guid id) => ProductToReturn;
        public Product Search(Guid id) => ProductToReturn;
    }

    [TestFixture]
    public class ProductControllerTests
    {
        private StubProductService _stubService;
        private ILogger<ProductController> _logger;
        private IConfiguration _config;
        private ProductController _controller;

        [SetUp]
        public void Setup()
        {
            _stubService = new StubProductService();
            _logger = new LoggerFactory().CreateLogger<ProductController>();
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "DataBaseFilePath", "fakePath" }
                })
                .Build();
            _controller = new ProductController(_logger, _stubService, _config);
        }

        [Test]
        public void GetProductID_ProductExists_ReturnsFoundMessage()
        {
            var id = Guid.NewGuid();
            _stubService.ProductToReturn = new Product { Id = id };

            var result = _controller.GetProductID(id);

            Assert.That(result, Is.EqualTo("Найден"));
        }

        [Test]
        public void GetProductID_ProductNotFound_ReturnsNotFoundMessage()
        {
            var id = Guid.NewGuid();
            _stubService.ProductToReturn = null;

            var result = _controller.GetProductID(id);

            Assert.That(result, Is.EqualTo("Продукт не найден"));
        }

        [Test]
        public void AddProduct_ValidProduct_ReturnsOkWithMessage()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "TestProduct" };
            _stubService.ProductToReturn = product;

            var result = _controller.AddProduct(product) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.ToString(), Does.Contain("Добавлен продукт"));
        }

        [Test]
        public void AddProduct_NullProduct_ReturnsBadRequest()
        {
            var result = _controller.AddProduct(null) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Продукт не добавлен"));
        }

        [Test]
        public void AddProduct_AddFails_ReturnsBadRequest()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "TestProduct" };
            _stubService.ProductToReturn = null;

            var result = _controller.AddProduct(product) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Продукт не добавлен"));
        }

        [Test]
        public void EditProduct_SuccessfulEdit_ReturnsOk()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "EditProduct", Definition = "desc", Price = 100 };
            _stubService.ProductToReturn = product;

            var result = _controller.EditProduct(product) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.ToString(), Does.Contain("Изменен продукт"));
        }

        [Test]
        public void EditProduct_EditFails_ReturnsBadRequest()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "EditFail" };
            _stubService.ProductToReturn = null;

            var result = _controller.EditProduct(product) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Продукт не изменен"));
        }

        [Test]
        public void RemoveProduct_SuccessfulRemoval_ReturnsOk()
        {
            var id = Guid.NewGuid();
            _stubService.ProductToReturn = new Product { Id = id, Name = "RemovedProduct" };

            var result = _controller.RemoveProduct(id) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo("Удален продукт"));
        }

        [Test]
        public void RemoveProduct_RemoveFails_ReturnsBadRequest()
        {
            var id = Guid.NewGuid();
            _stubService.ProductToReturn = null;

            var result = _controller.RemoveProduct(id) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Продукт не удален"));
        }
    }
}
