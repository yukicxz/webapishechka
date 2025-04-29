using Castle.Core.Configuration;
using Controllers;
using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using static NUnit.Framework.Assert;

namespace MagazineTests.Mock
{
    [TestFixture]
    public class ProductControllerMockTests
    {
        private Mock<ILogger<ProductController>> _mockLogger;
        private Mock<IProductService> _mockProductService;
        private ProductController _controller;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ProductController>>();
            _mockProductService = new Mock<IProductService>();

            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s.Value).Returns("dummyPath");

            var mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            mockConfig.Setup(c => c.GetSection("DataBaseFilePath")).Returns(mockSection.Object);


            _controller = new ProductController(_mockLogger.Object, _mockProductService.Object, mockConfig.Object);
        }

        [Test]
        public void GetProductID_ProductExists_ReturnsFoundMessage()
        {
            var id = Guid.NewGuid();
            _mockProductService.Setup(s => s.Search(id)).Returns(new Product { Id = id });

            var result = _controller.GetProductID(id);

            That(result, Is.EqualTo("Найден"));
        }

        [Test]
        public void GetProductID_ProductDoesNotExist_ReturnsNotFoundMessage()
        {
            var id = Guid.NewGuid();
            _mockProductService.Setup(s => s.Search(id)).Returns((Product)null);

            var result = _controller.GetProductID(id);

            That(result, Is.EqualTo("Продукт не найден"));
        }

        [Test]
        public void AddProduct_NullProduct_ReturnsBadRequest()
        {
            var result = _controller.AddProduct(null) as BadRequestObjectResult;

            That(result, Is.Not.Null);
            That(result!.StatusCode, Is.EqualTo(400));
            That(result.Value, Is.EqualTo("Продукт не добавлен"));
        }

        [Test]
        public void AddProduct_ValidProduct_ReturnsOk()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "TestProduct" };
            _mockProductService.Setup(s => s.Add(product)).Returns(product);

            var result = _controller.AddProduct(product) as OkObjectResult;

            That(result, Is.Not.Null);
            That(result!.StatusCode, Is.EqualTo(200));
            That(result.Value.ToString(), Does.Contain("Добавлен продукт"));
        }

        [Test]
        public void AddProduct_AddFails_ReturnsBadRequest()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "TestProduct" };
            _mockProductService.Setup(s => s.Add(product)).Returns((Product)null);

            var result = _controller.AddProduct(product) as BadRequestObjectResult;

            That(result, Is.Not.Null);
            That(result!.StatusCode, Is.EqualTo(400));
            That(result.Value, Is.EqualTo("Продукт не добавлен"));
        }

        [Test]
        public void EditProduct_ValidProduct_ReturnsOk()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Test", Definition = "Desc", Price = 10 };
            _mockProductService.Setup(s => s.Edit(product)).Returns(product);

            var result = _controller.EditProduct(product) as OkObjectResult;

            That(result, Is.Not.Null);
            That(result!.StatusCode, Is.EqualTo(200));
            That(result.Value.ToString(), Does.Contain("Изменен продукт"));
        }

        [Test]
        public void EditProduct_EditFails_ReturnsBadRequest()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Test" };
            _mockProductService.Setup(s => s.Edit(product)).Returns((Product)null);

            var result = _controller.EditProduct(product) as BadRequestObjectResult;

            That(result, Is.Not.Null);
            That(result!.StatusCode, Is.EqualTo(400));
            That(result.Value, Is.EqualTo("Продукт не изменен"));
        }

        [Test]
        public void RemoveProduct_ValidId_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var removedProduct = new Product { Id = id, Name = "Test" };
            _mockProductService.Setup(s => s.Remove(id)).Returns(removedProduct);

            var result = _controller.RemoveProduct(id) as OkObjectResult;

            That(result, Is.Not.Null);
            That(result!.StatusCode, Is.EqualTo(200));
            That(result.Value, Is.EqualTo("Удален продукт"));
        }

        [Test]
        public void RemoveProduct_RemoveFails_ReturnsBadRequest()
        {
            var id = Guid.NewGuid();
            _mockProductService.Setup(s => s.Remove(id)).Returns((Product)null);

            var result = _controller.RemoveProduct(id) as BadRequestObjectResult;

            That(result, Is.Not.Null);
            That(result!.StatusCode, Is.EqualTo(400));
            That(result.Value, Is.EqualTo("Продукт не удален"));
        }
    }
}
