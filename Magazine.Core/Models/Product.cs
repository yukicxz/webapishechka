using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Core.Models
{
    public class Product
    {

        private Guid id;
        private string name;
        private string definition;
        private decimal price;
        private string image;

        public Product(Guid id, string name, string definition, decimal price, string image)
        {
            this.id = id;
            this.name = name;
            this.definition = definition;
            this.price = price;
            this.image = image;
        }
        /// <summary>
        /// unique Id in system
        /// </summary>
        public Guid Id { get => id; set => id = value; }
        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get => name; set => name = value; }
        /// <summary>
        /// definition of product
        /// </summary>
        public string Definition { get => definition; set => definition = value; }
        /// <summary>
        /// Recommend price for product
        /// </summary>
        public decimal Price { get => price; set => price = value; }
        /// <summary>
        /// product image
        /// </summary>
        public string Image { get => image; set => image = value; }
    }
}
