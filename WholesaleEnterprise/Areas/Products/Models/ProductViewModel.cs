using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WholesaleEnterprise.Models;
namespace WholesaleEnterprise.Areas.Products.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
    }

    public class CreateProductViewModel
    {
        public string Currency { get; set; }
    }
    //public class RetailProductViewModel : ProductViewModel
    //{
    //    public Retailer Retailer { get; set; }
    //}

    //public class WholesaleProductViewModel : ProductViewModel
    //{
    //    public Wholesaler Wholesaler { get; set; }
    //}
}