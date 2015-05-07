using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNet.Mvc;
using UniversalStockMarketApp.Web.Models;

namespace UniversalStockMarketApp.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult ExchangeCourses()
        {
            XDocument yesterdayRates =
                XDocument.Load($@"http://www.nbrb.by/Services/XmlExRates.aspx?ondate={(DateTime.Today - new TimeSpan(1,0,0,0)).ToString("d")}");
            XDocument todayRates =
                XDocument.Load($@"http://www.nbrb.by/Services/XmlExRates.aspx?ondate={DateTime.Today.ToString("d")}");
            XDocument tommorowRates =
                XDocument.Load($@"http://www.nbrb.by/Services/XmlExRates.aspx?ondate={(DateTime.Today + new TimeSpan(1,0,0,0)).ToString("d")}");

            if (!tommorowRates.Root.IsEmpty)
            {
                yesterdayRates = todayRates;
                todayRates = tommorowRates;
                ViewBag.IsTommorowRatesAvailable = true;
            }
            else
                ViewBag.IsTommorowRatesAvailable = false;

            List<CurrencyModel> currencyCourses = new List<CurrencyModel>();

            currencyCourses.AddRange(todayRates.Root.Elements("Currency").Select(c => new CurrencyModel
            {
                CharCode = c.Element(nameof(CurrencyModel.CharCode))?.Value,
                Date = todayRates.Root.Attribute(nameof(CurrencyModel.Date))?.Value,
                Name = c.Element(nameof(CurrencyModel.Name))?.Value,
                NumCode = c.Element(nameof(CurrencyModel.NumCode))?.Value,
                Rate = c.Element(nameof(CurrencyModel.Rate))?.Value,
                Scale = c.Element(nameof(CurrencyModel.Scale))?.Value
            }).ToArray());

            foreach (CurrencyModel currency in currencyCourses)
            {
                var yesterdayRate = yesterdayRates.Root.Elements("Currency")
                    .FirstOrDefault(
                        y =>
                            string.Equals(y.Element(nameof(CurrencyModel.CharCode))?.Value,
                                currency.CharCode));
                currency.IsRisedInRelateToYesterday = double.Parse(currency.Rate, NumberStyles.Any) -
                                                      double.Parse(yesterdayRate?.Element(nameof(CurrencyModel.Rate))?.Value, NumberStyles.Any) > 0;
            }

            return View(currencyCourses);
        }
    }
}