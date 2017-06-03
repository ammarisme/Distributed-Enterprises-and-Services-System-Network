using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailTradingPortal.Models;

namespace RetailTradingPortal.Areas.Default.Models
{
    public class EnterprisePageViewModel
    {
        public IEnumerable<Enterprise> Enterprises { get; set; }
    }

    public class ProductViewModel
    {
        public Product Product { get; set; }

        public Enterprise Enterprise { get; set; }
    }

    public class ProductsByCategoryViewModel   
    {
        public IEnumerable<Product> Products { get; set; }

        public string Category { get; set; }
    }
    public class ProductsSerchViewModel
    {
        public IEnumerable<Product> Products { get; set; }

        public string SearchTerm { get; set; }
    }

    public class PlaceOrderViewModel {
        public Enterprise Enterprise { get; set; }

        public List<PaymentMethod> PaymentMethod { get; set; }

        public List<DeliveryMethod> DeliveryMode { get; set; }

        public Account Account { get; set; }
    }

    public class DeliveryMethod
    {
        public string DeliveryModeName { get; set; }
    }
    public class PaymentMethod
    {
        public string PaymentMethodName { get; set; }
    }
}