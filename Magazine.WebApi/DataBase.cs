using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using Magazine.Core.Models;

namespace Magazine.WebApi
{
    public class DataBase
    {
        private readonly string _connectionString;

        public DataBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        // SQL-запросы как константы
        private const string CREATE_TABLE = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id TEXT PRIMARY KEY,
                Definition TEXT NOT NULL,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                Image BLOB
            );";

        private const string CREATE_INDEX = @"
            CREATE INDEX IF NOT EXISTS idx_products_id ON Products(Id);";

        private const string INSERT_PRODUCT = @"
            INSERT INTO Products (Id, Definition, Name, Price, Image)
            VALUES (@Id, @Definition, @Name, @Price, @Image);";

        private const string SELECT_PRODUCT = @"
            SELECT Id, Definition, Name, Price, Image FROM Products
            WHERE Id = @Id;";

        private const string UPDATE_PRODUCT = @"
            UPDATE Products SET Definition = @Definition,
                                Name = @Name,
                                Price = @Price,
                                Image = @Image
            WHERE Id = @Id;";

        private const string DELETE_PRODUCT = @"
            DELETE FROM Products WHERE Id = @Id;";

        public void InitDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = CREATE_TABLE;
            tableCmd.ExecuteNonQuery();

            using var indexCmd = connection.CreateCommand();
            indexCmd.CommandText = CREATE_INDEX;
            indexCmd.ExecuteNonQuery();
        }

        public void InsertProduct(Product product)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = INSERT_PRODUCT;
            cmd.Parameters.AddWithValue("@Id", product.Id);
            cmd.Parameters.AddWithValue("@Definition", product.Definition);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Image", product.Image);
            cmd.ExecuteNonQuery();
        }

        public Product? SelectProduct(Guid id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = SELECT_PRODUCT;
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Product(
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDecimal(3),
                    reader.GetString(4)
                )
                {
                    Id = reader.GetGuid(0)
                };
            }
            return null;
        }

        public void UpdateProduct(Product product)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = UPDATE_PRODUCT;
            cmd.Parameters.AddWithValue("@Id", product.Id);
            cmd.Parameters.AddWithValue("@Definition", product.Definition);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Image", product.Image);
            cmd.ExecuteNonQuery();
        }

        public void DeleteProduct(Guid id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = DELETE_PRODUCT;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
