using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WholesaleEnterprise.Models
{
    [Table("Retailers")]
    public class Retailer
    {
        [Key]
        public string RetailerId { get; set; }

        public string RetailerName { get; set; }

        public string RetailerAddress { get; set; }

        public float?  Rating { get; set; }

        public string BusinessPhoneNumber { get; set; }

        public string Status { get; set; }

        public string BRCNumber { get; set; }

        public string Category { get; set; }

        public string Currency { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public ICollection<Quotation> Quotations { get; set; }

    }
}