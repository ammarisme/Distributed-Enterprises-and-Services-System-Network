using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailEnterprise.Models ;
using System.ComponentModel.DataAnnotations;

namespace RetailEnterprise.Areas.Stocks.Models
{
    public class CreateProductStocksViewModel
    {
        [Display(Name="Choose Product")]
        public IEnumerable<Product> ProductId;
    }

    public class StockDeductionViewModel
    {
        [Display(Name="Choose Product")]
        public IEnumerable<Product> ProductId;
    }

}

