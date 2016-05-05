using System;
using System.Collections.Generic;
using Lisb.Entity;

namespace Lisb.Repository
{
    public static class ProductRepository
    {
        static List<Product> _objList;
        public static IEnumerable<Product> GetData(int pageIndex, int pageSize)
        {
            int startAt = (pageIndex - 1) * pageSize;
            _objList = new List<Product>();
            Random r = new Random();
            int n = r.Next(1, 7);
            for (int i = startAt; i < startAt + pageSize; i++)
            {
                n = r.Next(1, 7);
                var obj = new Product
                {
                    Url = String.Format("http://dummyimage.com/150x{1}/{0}{0}{0}/fff.png&text={2}", n, n*50, i + 1),
                    Description = "Description of product " + (i + 1).ToString()
                };
                _objList.Add(obj);
            }
            return _objList;
        }
    }
}
