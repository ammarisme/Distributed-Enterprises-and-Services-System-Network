using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntegrationSystem.Models
{
    [Table("EnterpriseTypes")]
    public class EnterpriseType
    {
        [Key]
        public int EnterpriseTypeId { get; set; }

        public string Name { get; set; }

        public bool? Status { get; set; }

        // navigation properties
        public ICollection<Enterprise> Enterprises { get; set; }
    }

    [Table("Enterprises")]
    public class Enterprise
    {
        [Key]
        public int EnterpriseId { get; set; }

        [ForeignKey("EnterpriseTypes")]
        public int EnterpriseTypeId { get; set; }

        public string EnterpriseName { get; set; }

        public string EntepriseAddress { get; set; }

        public float? Rating { get; set; }

        public string BusinessPhoneNumber { get; set; }

        public bool Status { get; set; }

        public string BRCNumber { get; set; }

        public string Category { get; set; }

        public string Currency { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string Uri { get; set; }

        public EnterpriseType EnterpriseTypes { get; set; }

    }
}
