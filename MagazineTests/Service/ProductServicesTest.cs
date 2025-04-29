using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Magazine.Core.Models;
using Magazine.WebApi;
using Microsoft.Data.Sqlite;

namespace Magazine.ServiceTests
{
    [TestFixture]
    public class TestsProductService
    {
        private ProductService _service;
        private string _dbPath;
        private string _filePath;

        [SetUp]
        public void Setup()
        {
            _dbPath = "test_service.db";
            _filePath = "test_products.json";

            var configValues = new Dictionary<string, string?>
            {
                { "DataBasePath", _filePath },
                { "ConnectionStrings:sqlite", $"Data Source={_dbPath}" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _service = new ProductService(config);
            _service.InitDatabase();
        }

        [Test]
        public void Add_Product_ShouldBeStored()
        {
            var product = new Product("Тест описание", "Тест имя", 123.45m, "image.jpg");

            var result = _service.Add(product);

            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            var fetched = _service.Search(result.Id);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched.Id, Is.EqualTo(product.Id));
            Assert.That(fetched.Definition, Is.EqualTo(product.Definition));
            Assert.That(fetched.Name, Is.EqualTo(product.Name));
            Assert.That(fetched.Price, Is.EqualTo(product.Price));
            Assert.That(fetched.Image, Is.EqualTo(product.Image));
        }


        [Test]
        public void Remove_Product_ShouldBeDeleted()
        {
            var product = new Product("Удалим", "Продукт", 10, "x.png");
            var added = _service.Add(product);

            var removed = _service.Remove(added.Id);

            Assert.That(removed, Is.Not.Null);
            var search = _service.Search(added.Id);
            Assert.That(search, Is.Null);
        }

        [Test]
        public void Edit_Product_ShouldUpdateFields()
        {
            var product = new Product("Описание", "Имя", 1.99m, "img.jpg");
            var added = _service.Add(product);

            added.Name = "Новое имя";
            added.Price = 77.77m;
            var updated = _service.Edit(added);

            Assert.That(updated.Name, Is.EqualTo("Новое имя"));
            Assert.That(updated.Price, Is.EqualTo(77.77m));

            var fetched = _service.Search(added.Id);
            Assert.That(fetched.Name, Is.EqualTo("Новое имя"));
        }

        [Test]
        public void Search_Product_ShouldReturnCorrectData()
        {
            var product = new Product("Найти", "Товар", 19.95m, "pic.jpg");
            var added = _service.Add(product);

            var result = _service.Search(added.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Товар"));
        }

        [Test]
        public void Search_NonexistentProduct_ShouldReturnNull()
        {
            var result = _service.Search(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }
        [Test]
        public void Add_And_Search_ShouldStoreAndRetrieveAllFields()
        {
            var product = new Product("Тест описание", "Тест имя", 123.45m, "image.jpg");

            var added = _service.Add(product);
            var fetched = _service.Search(added.Id);

            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched.Id, Is.EqualTo(added.Id));
            Assert.That(fetched.Definition, Is.EqualTo(product.Definition));
            Assert.That(fetched.Name, Is.EqualTo(product.Name));
            Assert.That(fetched.Price, Is.EqualTo(product.Price));
            Assert.That(fetched.Image, Is.EqualTo(product.Image));
        }
        [Test]
        public void FileSerialization_ShouldPersistData()
        {
            var product = new Product("Сер", "Товар", 9.99m, "x.png");
            var added = _service.Add(product);

            // Проверяем содержимое файла
            var fileContent = File.ReadAllText("test_products.json");
            Console.WriteLine(fileContent);

            // Эмуляция перезапуска
            var configValues = new Dictionary<string, string?>
    {
        { "DataBasePath", _filePath },
        { "ConnectionStrings:sqlite", $"Data Source={_dbPath}" }
    };
            var config = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
            var newService = new ProductService(config);
            newService.InitDatabase();

            var result = newService.Search(added.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Товар"));
        }
        [Test]
        public void Index_ShouldExist_OnProductsId()
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"PRAGMA index_list(Products);";

            using var reader = command.ExecuteReader();

            var indexExists = false;
            while (reader.Read())
            {
                var indexName = reader.GetString(1); // Второй столбец — имя индекса
                if (indexName == "idx_products_id")
                {
                    indexExists = true;
                    break;
                }
            }

            Assert.That(indexExists, Is.True, "Индекс 'idx_products_id' не найден в таблице Products.");
        }
    }
}
