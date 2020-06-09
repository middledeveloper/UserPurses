using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace UserPurses.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Rate { get; set; }

        public static IList<Currency> Import(string xml)
        {
            var currencies = new List<Currency>();
            var doc = new XmlDocument();

            try
            {
                doc.Load(xml);
            }
            catch(System.Net.WebException)
            {
                return null;
            }

            var nodes = doc.SelectNodes("//*[@currency]");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    var item = new Currency()
                    {
                        Title = node.Attributes["currency"].Value,
                        Rate = Decimal.Parse(node.Attributes["rate"].Value, NumberStyles.Any, new CultureInfo("en-Us"))
                    };

                    currencies.Add(item);
                }
            }

            return currencies;
        }

        public static string ConvertToStr(decimal amount)
        {
            return String.Format("{0:0.00}", amount);
        }
    }
}
