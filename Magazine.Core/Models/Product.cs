using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Core.Models
{
    public class Product
    {

        /// <summary>
        /// unique Id in system
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// definition of product
        /// </summary>
        public string Definition { get; set; }
        /// <summary>
        /// Recommend price for product
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// product image
        /// </summary>
        public string Image { get; set; }

        public Product() { }
        public Product(string name, string definition, decimal price, string image)
        {
            Id = Guid.NewGuid();
            Name = name;
            Definition = definition;
            Price = price;
            Image = image;
        }

    }
}
