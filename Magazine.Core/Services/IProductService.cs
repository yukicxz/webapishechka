using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magazine.Core.Models;

namespace Magazine.Core.Services
{
    public interface IProductService
    {
        /// <summary>
        /// add element to db
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Product Add(Product product);
        /// <summary>
        /// remove element from db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Product Remove(Guid id);
        /// <summary>
        /// edit element in db
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Product Edit(Product product);
        /// <summary>
        /// search element in db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Product? Search(Guid id);
    }
}
