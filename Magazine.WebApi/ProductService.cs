using Microsoft.Data.Sqlite;
using Magazine.Core.Services;
using Magazine.Core.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
namespace Magazine.WebApi
{
    public class ProductService : IProductService
    {
        private readonly string _connection;
        private readonly string _filePath;
        private readonly IConfiguration _config;
        private readonly Dictionary<Guid, Product> _products = new();
        private readonly Mutex _mutex = new();
        public ProductService(IConfiguration config)
        {
            _config = config;
            _connection = config.GetConnectionString("sqlite");
            _filePath = config["DataBasePath"] ?? "database.txt";
            InitFromFile();
            InitDatabase();

        }

        private void InitDatabase()
        {
            using var connection = new SqliteConnection(_connection);
            connection.Open();
            Console.WriteLine("Database opened successfully."); // Debug

            var command = connection.CreateCommand();
            command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Products(
            Id TEXT PRIMARY KEY,
            Definition TEXT NOT NULL,
            Name TEXT NOT NULL,
            Price REAL NOT NULL,
            Image BLOB
            );";

            command.ExecuteNonQuery();
            Console.WriteLine("Table check/creation completed."); // Debug
        }
        public Product Add(Product product)
        {
            if (product.Id == Guid.Empty)
            {
                product.Id = Guid.NewGuid();
            }

            using var connection = new SqliteConnection(_connection);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT INTO Products (Id, Definition, Name, Price, Image)
        VALUES ($id, $definition, $name, $price, $image);
    ";
            command.Parameters.AddWithValue("$id", product.Id);
            command.Parameters.AddWithValue("$definition", product.Definition);
            command.Parameters.AddWithValue("$name", product.Name);
            command.Parameters.AddWithValue("$price", product.Price);
            command.Parameters.AddWithValue("$image", product.Image);
            command.ExecuteNonQuery();

            return product;
        }
        public Product Remove(Guid productID)
        {
            Product RemoveProduct = Search(productID);
            if (RemoveProduct != null)
            {
                using var connection = new SqliteConnection(_connection);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                DELETE FROM Products WHERE Id = $id;
                ";
                command.Parameters.AddWithValue("$id", productID);
                command.ExecuteNonQuery();
            }
            return RemoveProduct;
        }

        public Product Edit(Product product)
        {
            using var connection = new SqliteConnection(_connection);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
            UPDATE Products SET Definition = $definition,
            Name = $name,
            Price = $price,
            Image = $image
            WHERE Id = $id;
            ";
            command.Parameters.AddWithValue("$id", product.Id);
            command.Parameters.AddWithValue("$definition", product.Definition);
            command.Parameters.AddWithValue("$name", product.Name);
            command.Parameters.AddWithValue("$price", product.Price);
            command.Parameters.AddWithValue("$image", product.Image);
            command.ExecuteNonQuery();
            return product;
        }

        public Product Search(Guid productID)
        {
            using var connection = new SqliteConnection(_connection);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Definition, Name, Price, Image FROM Products WHERE Id = $id;
                ";
            command.Parameters.AddWithValue("$id", productID);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                (
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDecimal(3),
                    reader.GetString(4)
                )
                {
                    Id = reader.GetGuid(0) // Устанавливаем Id отдельно, так как конструктор создаёт новый Id
                };
            }
            return null;
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

    }
}
