using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Magazine.WebApi
{
    public class ProductService : IProductService
    {
        private readonly string _filePath;
        private readonly IConfiguration _config;
        private readonly Dictionary<Guid, Product> _products = new();
        private readonly Mutex _mutex = new();
        private readonly DataBase _db;

        public ProductService(IConfiguration config)
        {
            _config = config;
            _filePath = config["DataBasePath"] ?? "database.txt";
            var connectionString = config.GetConnectionString("sqlite") ?? "Data Source=products.db";
            _db = new DataBase(connectionString);

            InitFromFile();
            _db.InitDatabase();
        }

        public Product Add(Product product)
        {
            if (product.Id == Guid.Empty)
                product.Id = Guid.NewGuid();

            _mutex.WaitOne();
            try
            {
                _db.InsertProduct(product);
                _products[product.Id] = product;
                WriteToFile();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return product;
        }

        public Product? Remove(Guid productID)
        {
            Product? toRemove = null;
            _mutex.WaitOne();
            try
            {
                toRemove = Search(productID);
                if (toRemove != null)
                {
                    _db.DeleteProduct(productID);
                    _products.Remove(productID);
                    WriteToFile();
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return toRemove;
        }

        public Product Edit(Product product)
        {
            _mutex.WaitOne();
            try
            {
                _db.UpdateProduct(product);
                _products[product.Id] = product;
                WriteToFile();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return product;
        }

        public Product? Search(Guid productID)
        {
            return _db.SelectProduct(productID);
        }

        private void InitFromFile()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("Product file not found. Creating a new file.");
                return;
            }
            try
            {
                var json = File.ReadAllText(_filePath);
                var deserializedProducts = JsonSerializer.Deserialize<Dictionary<Guid, Product>>(json);
                if (deserializedProducts != null)
                {
                    foreach (var product in deserializedProducts)
                    {
                        _products[product.Key] = product.Value;
                    }
                    Console.WriteLine("Продукты загружены из файла.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка загрузки из файла: " + ex.Message);
            }
        }

        private void WriteToFile()
        {
            try
            {
                var text = JsonSerializer.Serialize(_products);
                File.WriteAllText(_filePath, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка записи в файл: " + ex.Message);
            }
        }
    }
}
