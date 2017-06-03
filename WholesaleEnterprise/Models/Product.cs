using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WholesaleEnterprise.Models
{

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string Unit { get; set; }

        public float? RetailPrice { get; set; }

        public float? WholesalePrice { get; set; }

        public float? StocksQuantity { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public ICollection<SpecificationInProduct> SpecificationInProduct { get; set; }

        public ICollection<ProductInPurchaseOrder> ProductInPurchaseOrders { get; set; }

        public ICollection<ProductInRetailSale> ProductInSales { get; set; }
        
    }
    public class SpecificationInProduct
    {
        public int SpecificationInProductId { get; set; }

        // foreign key in product
        public int ProductId { get; set; }

        public string Specification { get; set; }

        public string Value { get; set; }
    }
}