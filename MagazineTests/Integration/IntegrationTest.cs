using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Controllers;
using Magazine.WebApi;
using Magazine.Core.Models;
using Microsoft.Data.Sqlite;

namespace Magazine.IntegrationTests
{
    [TestFixture]
    public class ProductControllerIntegrationTests
    {
        private ProductController _controller;
        private ProductService _service;

        [SetUp]
        public void Setup()
        {
            var configValues = new Dictionary<string, string>
            {
                { "DataBasePath", "test_products.json" },
                { "ConnectionStrings:sqlite", "DataSource=test.db" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _service = new ProductService(config);
            _service.InitDatabase(); // обязательно вызвать вручную!
            _controller = new ProductController(NullLogger<ProductController>.Instance, _service, config);
        }

        [Test]
        public void AddAndGetProduct_IntegrationTest()
        {
            var product = new Product("Описание", "Имя", 100, "img.jpg");

            var addResult = _controller.AddProduct(product) as OkObjectResult;
            Assert.That(addResult, Is.Not.Null);

            var msg = addResult.Value.ToString();
            Guid.TryParse(msg.Split("id: ")[1], out Guid id);
            Assert.That(id, Is.Not.EqualTo(Guid.Empty));

            var getResult = _controller.GetProductID(id);
            Assert.That(getResult, Is.EqualTo("Найден"));
        }

        [Test]
        public void AddEditGetProduct_IntegrationTest()
        {
            var product = new Product("Описание", "Имя", 100, "img.jpg");

            var addResult = _controller.AddProduct(product) as OkObjectResult;
            Guid.TryParse(addResult.Value.ToString().Split("id: ")[1], out Guid id);
            product.Id = id;

            product.Name = "Обновлённое имя";
            product.Price = 999;

            var editResult = _controller.EditProduct(product) as OkObjectResult;
            Assert.That(editResult, Is.Not.Null);

            var getResult = _controller.GetProductID(product.Id);
            Assert.That(getResult, Is.EqualTo("Найден"));
        }

        [Test]
        public void AddAndRemoveProduct_IntegrationTest()
        {
            var product = new Product("Кратко", "Удалим", 222, "del.jpg");

            var addResult = _controller.AddProduct(product) as OkObjectResult;
            Guid.TryParse(addResult.Value.ToString().Split("id: ")[1], out Guid id);

            var removeResult = _controller.RemoveProduct(id) as OkObjectResult;
            Assert.That(removeResult.Value, Is.EqualTo("Удален продукт"));

            var getResult = _controller.GetProductID(id);
            Assert.That(getResult, Is.EqualTo("Продукт не найден"));
        }
        [Test]
        public void Index_ShouldExist_AfterInitDatabase()
        {
            using var conn = new SqliteConnection("Data Source=test.db"); // путь должен совпадать
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA index_list('Products')";

            using var reader = cmd.ExecuteReader();

            bool found = false;
            while (reader.Read())
            {
                var name = reader.GetString(1); // 1 — это имя индекса
                if (name == "idx_products_id") // замените на фактическое имя индекса
                {
                    found = true;
                    break;
                }
            }

            Assert.That(found, Is.True, "Ожидался индекс 'idx_products_id', но он не найден");
        }

    }
}
