using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace MvcApplication1.App_Start
{
    public static class DummyDataSeeder
    {
        public static void Seed()
        {
            using (var db = new actionDbContext())
            {
                var existingIds = new HashSet<string>(
                    db.products
                        .Where(p => p.productID != null)
                        .Select(p => p.productID)
                        .ToList(),
                    StringComparer.OrdinalIgnoreCase);

                var hasChanges = false;
                var samples = BuildSampleProducts();
                foreach (var item in samples)
                {
                    if (existingIds.Contains(item.productID))
                    {
                        continue;
                    }

                    db.products.Add(item);
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    db.SaveChanges();
                }
            }
        }

        private static IEnumerable<Product> BuildSampleProducts()
        {
            var now = DateTime.Now.ToString("yyyy-MM-dd");

            return new List<Product>
            {
                CreateProduct("EA-1001", "Vintage Wrist Watch", "HomeDecor", "Classic collector watch in polished steel.", 4200, 5200, 140, "seller_demo_1", "p1.jpg", now),
                CreateProduct("EA-1002", "Handmade Brass Lamp", "HomeDecor", "Warm ambient desk lamp with artisan finish.", 3000, 3600, 120, "seller_demo_2", "p2.jpg", now),
                CreateProduct("EA-1003", "Executive Leather Chair", "Office", "Comfort-focused premium chair for workspaces.", 7000, 8400, 180, "seller_demo_3", "p3.jpg", now),
                CreateProduct("EA-1004", "Premium Tea Set", "Kitchen", "Ceramic tea set for six with gift-ready box.", 2500, 3100, 160, "seller_demo_4", "p4.jpg", now),
                CreateProduct("EA-1005", "Govt Surplus Laptop", "GovernmentProduct", "Tested refurbished device from office stock.", 12000, 13500, 200, "seller_demo_5", "p5.jpg", now),
                CreateProduct("EA-1006", "Official Filing Cabinet", "GovernmentProduct", "Heavy duty steel cabinet with lock system.", 6500, 7200, 170, "seller_demo_6", "p6.jpg", now),
                CreateProduct("EA-1007", "Public Library Bookshelf", "GovernmentProduct", "Hardwood bookshelf with reinforced frame.", 5400, 6100, 190, "seller_demo_7", "p7.jpg", now),
                CreateProduct("EA-1008", "Municipal Conference Table", "GovernmentProduct", "Large boardroom table in excellent condition.", 15000, 16200, 220, "seller_demo_8", "p8.jpg", now),
                CreateProduct("EA-1009", "Minimal Ceramic Vase", "HomeDecor", "Blue-white handcrafted vase for living spaces.", 2200, 2800, 130, "seller_demo_9", "p9.jpg", now),
                CreateProduct("EA-1010", "Collector Pop Art Frame", "Art", "Gallery-style print with matte finish.", 1800, 2300, 110, "seller_demo_10", "p10.jpg", now),
                CreateProduct("EA-1011", "Classic Gramophone", "Collectibles", "Restored gramophone inspired decor piece.", 6500, 8100, 210, "seller_demo_11", "p11.jpg", now),
                CreateProduct("EA-1012", "Ceramic Storage Jar", "Kitchen", "Traditional glazed jar with lid.", 1400, 1750, 100, "seller_demo_12", "p12.jpg", now),
                CreateProduct("EA-1013", "Industrial Desk Lamp", "Office", "Metal body lamp with adjustable neck.", 2900, 3500, 145, "seller_demo_13", "p2.jpg", now),
                CreateProduct("EA-1014", "Wooden Wall Clock", "HomeDecor", "Retro analog wall clock in oak finish.", 2300, 2900, 135, "seller_demo_14", "p3.jpg", now),
                CreateProduct("EA-1015", "Limited Edition Wall Art", "Art", "Statement art print for modern interiors.", 3600, 4500, 175, "seller_demo_15", "p1.jpg", now),
                CreateProduct("EA-1016", "Smartphone Bundle", "Electronics", "Refurbished smartphone with charger and case.", 9800, 11100, 165, "seller_demo_16", "p5.jpg", now),
                CreateProduct("EA-1017", "Mechanical Keyboard", "Electronics", "Compact keyboard with tactile switches.", 4800, 5600, 125, "seller_demo_17", "p6.jpg", now),
                CreateProduct("EA-1018", "Executive Briefcase", "Fashion", "Premium leather briefcase for daily office use.", 5200, 6000, 150, "seller_demo_18", "p7.jpg", now),
                CreateProduct("EA-1019", "Sport Chronograph", "Fashion", "Water-resistant chronograph for active lifestyle.", 6100, 6900, 155, "seller_demo_19", "p8.jpg", now),
                CreateProduct("EA-1020", "Portable Air Purifier", "Electronics", "Desktop purifier ideal for small rooms.", 4300, 5100, 140, "seller_demo_20", "p9.jpg", now)
            };
        }

        private static Product CreateProduct(
            string productCode,
            string productName,
            string category,
            string details,
            double basePrice,
            double biddingPrice,
            int maxTime,
            string seller,
            string imageFileName,
            string date)
        {
            return new Product
            {
                productID = productCode,
                ProductName = productName,
                Category = category,
                Details = details,
                FileName = imageFileName,
                ImageData = LoadImageOrFallback(imageFileName),
                Status = "live",
                MaxTime = maxTime,
                BiddingTime = maxTime,
                CountClick = 0,
                BasePrice = basePrice,
                BiddingPrice = biddingPrice,
                BuyerName = string.Empty,
                SellerName = seller,
                Date = date
            };
        }

        private static byte[] LoadImageOrFallback(string imageFileName)
        {
            var path = HostingEnvironment.MapPath("~/Content/mytemplate/images/home/" + imageFileName);
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            Debug.WriteLine("DummyDataSeeder: missing image file " + imageFileName);
            return new byte[] { 0x00 };
        }
    }
}
