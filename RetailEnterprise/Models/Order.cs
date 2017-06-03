using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RetailEnterprise.Models
{
    public class Order
    {
        [Key]
            public int OrderId { get; set; }

            public DateTime? OrderDate { get; set; }

            public DateTime? OrderDueDate { get; set; }

            public string OrderStatus { get; set; }
            [NotMapped]
            public string OrderStatusId { get; set; }

            public DateTime? DeliveredDate { get; set; }

            public string DeliveryStatus { get; set; }
            [NotMapped]
            public string DeliverStatusId { get; set; }

            public string PaymentMethod { get; set; }
            [NotMapped]
            public string PaymentMethodId { get; set; }

            public int PaymentDuration { get; set; }

            public string DeliveryMode { get; set; }
            [NotMapped]
            public string DeliveryModeId { get; set; }

            public int ServiceId { get; set; }

            public string Remark { get; set; }

            public int? ServiceOrderId { get; set; }
    }

    [Table("PurchaseOrders")]
    public class PurchaseOrder : Order
    {
        [ForeignKey("Suppliers")]
        public string SupplierId { get; set; }

        // navigational properties
        public ICollection<ProductInPurchaseOrder> ProductsInPurchaseOrders { get; set; }

        public Account Accounts { get; set; }

        public Supplier Suppliers { get; set; }
    }

    public class ProductInOrder
    {
        [ForeignKey("Products")]
        public int ProductId { get; set; }

        public float Quantity { get; set; }

        public string Remark { get; set; }

        public Product Products { get; set; }
    }
    [Table("ProductsInPurchaseOrders")]
    public class ProductInPurchaseOrder : ProductInOrder
    {
        [Key]
        public int ProductInPurchaseOrderId { get; set; }

        public float Cost { get; set; }

        [ForeignKey("PurchaseOrders")]
        public int PurchaseOrderId { get; set; }

        public PurchaseOrder PurchaseOrders { get; set; }
    }

    // Sales tables and Models

    [Table("Sales")]
    public class Sale : Order
    {
        //foreign key to Customers
        [ForeignKey("Customers")]
        public int CustomerId { get; set; }

        public ICollection<ProductInSale> ProductsInRetailOrder { get; set; }

        public Customer Customers { get; set; }

        public Account Accounts { get; set; }
    }

    [Table("ProductsInSales")]
    public class ProductInSale : ProductInOrder
    {
        [Key]
        public int ProductInSaleId { get; set; }

        public float UnitPrice { get; set; }

        [ForeignKey("Sales")]
        public int SaleId { get; set; }

        public Sale Sales { get; set; }
    }

    [Table("ProductsInSaleReturns")]
    public class ProductInSaleReturn 
    {
        [Key]
        public int ProductInSaleReturnId { get; set; }
        
        [ForeignKey("Sales")]
        public int SaleId { get; set; }

        public Sale Sales { get; set; }
    }

}
