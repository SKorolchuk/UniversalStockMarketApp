using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversalStockMarketApp.Web.Models
{
    public class CurrencyModel
    {
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public string Scale { get; set; }
        public string Name { get; set; }
        public string Rate { get; set; }
        public string Date { get; set; }
        public bool IsRisedInRelateToYesterday { get; set; }
    }
}
