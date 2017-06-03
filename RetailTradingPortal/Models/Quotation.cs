using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetailTradingPortal;

namespace RetailTradingPortal.Models
{
    [Table("Quotations")]
    public class Quotation
    {
        [Key]
        public int QuotationId { get; set; }

        // foreign keys are mapped in the context class
        // foreign keys are mapped in the context class

        [ForeignKey("Enterprises")]
        public int EnterpriseId { get; set; }
        
        public string Status { get; set; }

        public string PaymentMethod { get; set; }

        public int PaymentDuration { get; set; }

        public string DeliveryMethod { get; set; }
        // navigational properties

        public virtual Enterprise Enterprises { get; set; }
        
        public ICollection<ProductInQuotation> ProductsInQuotation { get; set; }

    }

    [Table("ProductsInQuotations")]
    public class ProductInQuotation
    {
        [Key]
        public int ProductInQuotationId { get; set; }

        [ForeignKey("Products")]
        public int ProductId { get; set; }

        [ForeignKey("Quotations")]
        public int QuotationId { get; set; }

        public int Quantity { get; set; }

        public float UnitPrice { get; set; }

        public virtual Product Products { get; set; }

        public virtual Quotation Quotations { get; set; }
    }
}
