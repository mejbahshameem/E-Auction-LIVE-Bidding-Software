using MvcApplication1.Models;
using MvcApplication1.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
  
    public class HomeController : Controller
    {
        public static int ibc = 0;
        
        private actionDbContext db = new actionDbContext();
        private readonly AppEmailService emailService = new AppEmailService();
        public static String pr1buyer=null;
        public static String pr2buyer = null;
        public static int i1 = 0;
        public static int i2 = 0;
        public static String pr3buyer = null;
        public static int i3 = 0;

        public ActionResult EditUser()
        {
            return View();
        }
        public ActionResult Deletecl()
        {
            String buy = (string)(Session["log"]);
            var item = db.reg.Where(v => v.UserName.Equals(buy));
            int ID=0;
            foreach(  var u in item)
            {
                ID=u.ID;
            }
            Regi r=db.reg.Find(ID);
            db.reg.Remove(r);
            db.SaveChanges();
            Session["log"] = null;
            return RedirectToAction("Index");
        }
        public ActionResult UserBuyInfo()
        {
            String buy = (string)(Session["log"]);
            var item = db.sales.Where(v => v.BuyerName.Equals(buy));
            var list = item.ToList();
            return View(list);
         
        }

        public ActionResult UserSoldProduct()
        {
            String buy = (string)(Session["log"]);
            var item = db.sales.Where(v => v.SellerName.Equals(buy));
            var list = item.ToList();
            return View(list);
         
        }
        [HttpPost]
        public ActionResult EditUser(Regi client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("myAccount");
            }
            return View(client);
        }
        public ActionResult myAccount()
        {
            String user = (string)(Session["log"]);
            if (!String.IsNullOrWhiteSpace(user))
            {
                ViewBag.BuyCount = db.sales.Count(v => v.BuyerName.Equals(user));
                ViewBag.SoldCount = db.sales.Count(v => v.SellerName.Equals(user));
                ViewBag.LiveCount = db.products.Count(v => v.SellerName.Equals(user) && v.Status != null && v.Status.Equals("live"));
                ViewBag.AlertCount = db.alerts.Count(v => v.UserName.Equals(user));
            }

            return View();
        }
        public ActionResult SearchGVTA()
        {
            return RedirectToAction("SearchByCategory", new { id = "GovernmentProduct" });
        }
        public ActionResult SearchHD()
        {
            return RedirectToAction("SearchByCategory", new { id = "HomeDecor" });
        }

        public ActionResult SearchByCategory(String id)
        {
            ViewBag.a = 1;
            var allProducts = db.products.ToList();
            PopulateCategorySummary(allProducts);

            if (String.IsNullOrWhiteSpace(id))
            {
                return View("Index", allProducts);
            }

            var list = allProducts.Where(v => v.Category != null && v.Category.Equals(id, StringComparison.OrdinalIgnoreCase)).ToList();
            ViewBag.SelectedCategory = id;
            if (!list.Any())
            {
                ViewBag.b = "No live Auction in your searched category.";
            }

            return View("Index", list);
        }
        public ActionResult Index()
        {
            ViewBag.a = 1;
            if (ibc == 1)
            {
                
                TempData["msg"] = "<script>alert('Your bid request has been received. You will be notified if bid is done. Thanks.');</script>";
                ViewBag.k = ibc;
                ibc = 0;
            }

            var products = db.products.OrderByDescending(v => v.ID).ToList();
            PopulateCategorySummary(products);
            return View(products);
        
        }

        private void PopulateCategorySummary(List<Product> products)
        {
            var categoryData = products
                .Where(v => !String.IsNullOrWhiteSpace(v.Category))
                .GroupBy(v => v.Category)
                .Select(v => new KeyValuePair<String, Int32>(v.Key, v.Count()))
                .OrderByDescending(v => v.Value)
                .ThenBy(v => v.Key)
                .ToList();

            ViewBag.CategoryData = categoryData;
        }

        private String GetEmailForUser(String userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            return db.reg
                .Where(v => v.UserName.Equals(userName))
                .Select(v => v.Email)
                .FirstOrDefault();
        }

        private void NotifyAuctionClosed(Product product, Sold soldProduct, String winnerUserName)
        {
            var winnerEmail = GetEmailForUser(winnerUserName);
            if (!String.IsNullOrWhiteSpace(winnerEmail))
            {
                emailService.TrySend(
                    winnerEmail,
                    "LIVE BID WINNER!",
                    "Your are the higest bidder for the product " + product.ProductName + ". Please contact us ASAP for delivery & payment procedure.Thanks");
            }

            var sellerEmail = GetEmailForUser(soldProduct.SellerName);
            if (!String.IsNullOrWhiteSpace(sellerEmail))
            {
                emailService.TrySend(
                    sellerEmail,
                    "LIVE BID PRODUCT SOLD!",
                    "Your product " + product.ProductName + " has been sold for the price " + soldProduct.Price + ". Please contact us ASAP for further Information.Thanks");
            }
        }
        
        public ActionResult updateBidTime(String timer,String id,String price,String counter) {
            float fl = float.Parse(price);
            int id1 = Int32.Parse(id);
            int id2 = Int32.Parse(timer);
        
            int cc = Int32.Parse(counter);
            Product prd = db.products.Find(id1);
            String buyer = (string)(Session["log"]);
            if (prd != null)
            {
               
              
                if (id2 >= 0)
                {
                  
                    prd.CountClick = cc;
                    prd.BiddingPrice = fl;
                    prd.BiddingTime = id2;
                    db.Entry(prd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (id2 == 0 && cc > 0)
                {

                    if (i1 == 0)
                    {
                        prd.Status = "sold";
                        String time = System.DateTime.Now.ToShortDateString();
                        prd.Date = time;
                        prd.BuyerName = pr1buyer;
                        prd.BiddingTime = 0;
                        db.Entry(prd).State = EntityState.Modified;
                        db.SaveChanges();
                        Sold soldproduct = new Sold();
                        soldproduct.BuyerName = pr1buyer;
                        soldproduct.productID = prd.productID;
                        soldproduct.SellerName = prd.SellerName;
                        soldproduct.FileName = prd.FileName;
                        soldproduct.ImageData = prd.ImageData;
                        soldproduct.Price = prd.BiddingPrice;
                        soldproduct.Date = prd.Date;
                        db.sales.Add(soldproduct);
                        db.SaveChanges();
                        i1 = 1;
                        NotifyAuctionClosed(prd, soldproduct, pr1buyer);

                    }
                }
                if (id2 == 0 && cc == 0)
                {
                    
                    prd.Status = "unsold";
                    db.Entry(prd).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
           
           
           
            return View();
        }

        public ActionResult updateBidTime1(String timer, String id, String price, String counter)
        {
            float fl = float.Parse(price);
            int id1 = Int32.Parse(id);
            int id2 = Int32.Parse(timer);

            int cc = Int32.Parse(counter);
            Product prd = db.products.Find(id1);
            String buyer = (string)(Session["log"]);
            if (prd != null)
            {


                if (id2 >= 0)
                {
                    prd.CountClick = cc;
                    prd.BiddingPrice = fl;
                    prd.BiddingTime = id2;
                    db.Entry(prd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (id2 == 0 && cc > 0)
                {

                    if (i2 == 0)
                    {

                        prd.Status = "sold";
                        String time = System.DateTime.Now.ToShortDateString();
                        prd.Date = time;
                        prd.BuyerName = pr2buyer;
                        prd.BiddingTime = 0;
                        db.Entry(prd).State = EntityState.Modified;
                        db.SaveChanges();
                        Sold soldproduct = new Sold();
                        soldproduct.BuyerName = pr2buyer;
                        soldproduct.productID = prd.productID;
                        soldproduct.SellerName = prd.SellerName;
                        soldproduct.FileName = prd.FileName;
                        soldproduct.ImageData = prd.ImageData;
                        soldproduct.Price = prd.BiddingPrice;
                        soldproduct.Date = prd.Date;
                        db.sales.Add(soldproduct);
                        db.SaveChanges();
                        i2 = 1;
                        NotifyAuctionClosed(prd, soldproduct, pr2buyer);

                    }
                }
                if (id2 == 0 && cc == 0)
                {
                    
                    prd.Status = "unsold";
                    db.Entry(prd).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }



            return View();
        }
        public ActionResult updateBidTime2(String timer2, String id2, String price2, String counter2)
        {
            float fl = float.Parse(price2);
            int id1 = Int32.Parse(id2);
            int id4 = Int32.Parse(timer2);

            int cc = Int32.Parse(counter2);
            Product prd = db.products.Find(id1);
            String buyer = (string)(Session["log"]);
            if (prd != null)
            {


                if (id4 >= 0)
                {
                    prd.CountClick = cc;
                    prd.BiddingPrice = fl;
                    prd.BiddingTime = id4;
                    db.Entry(prd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (id4 == 0 && cc > 0)
                {

                    if (i3 == 0)
                    {

                        prd.Status = "sold";
                        String time = System.DateTime.Now.ToShortDateString();
                        prd.Date = time;
                        prd.BuyerName = pr3buyer;
                        prd.BiddingTime = 0;
                        db.Entry(prd).State = EntityState.Modified;
                        db.SaveChanges();
                        Sold soldproduct = new Sold();
                        soldproduct.BuyerName = pr3buyer;
                        soldproduct.productID = prd.productID;
                        soldproduct.SellerName = prd.SellerName;
                        soldproduct.FileName = prd.FileName;
                        soldproduct.ImageData = prd.ImageData;
                        soldproduct.Price = prd.BiddingPrice;
                        soldproduct.Date = prd.Date;
                        db.sales.Add(soldproduct);
                        db.SaveChanges();
                        i3 = 1;
                        NotifyAuctionClosed(prd, soldproduct, pr3buyer);

                    }
                }
                if (id4 == 0 && cc == 0)
                {

                    prd.Status = "unsold";
                    db.Entry(prd).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }



            return View();
        }
    
        public ActionResult REsetBidTime(String id1)
        {

            int id3 = Int32.Parse(id1);
            Product prd = db.products.Find(id3);
            if (prd != null)
            {
                String buyer = (string)(Session["log"]);
                pr1buyer = buyer;
            }
            return View();
                    
        }
        public ActionResult REsetBidTime1(String id1)
        {

            int id3 = Int32.Parse(id1);
            Product prd = db.products.Find(id3);
            if (prd != null)
            {
                String buyer = (string)(Session["log"]);
                pr2buyer = buyer;
            }
            return View();

        }
        public ActionResult REsetBidTime2(String id5)
        {

            int id3 = Int32.Parse(id5);
            Product prd = db.products.Find(id3);
            if (prd != null)
            {
                String buyer = (string)(Session["log"]);
                pr3buyer = buyer;
            }
            return View();

        }
        public ActionResult login()
        {

           
            return View();
        }
        [HttpPost]
        public ActionResult login(String uname, String password)
        {
            int i = 0;
            ViewBag.error2 = 0;
            String time1 = System.DateTime.Now.ToShortDateString();
            if (Request.Form["submit1"] != null)
            {

                List<Regi> item1 = db.reg.ToList();
                foreach (var d in item1)
                {
                    if (d.UserName.Equals(uname) && d.Password.Equals(password))
                    {
                        String time = System.DateTime.Now.ToShortDateString();
                        i=1;
                        System.Diagnostics.Debug.WriteLine(time);
                        Session["log"] = uname;
                        string buyer = (string)(Session["log"]);
                        System.Diagnostics.Debug.WriteLine(buyer);
                        return RedirectToAction("Index");
                    }

                }
                if (i == 0)
                {
                    ViewBag.error5 = 0;
                    return View();
                    i = 5;
                }
                
                   
                  
                }
            

            return View();
        }
        public ActionResult RequestForBid()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RequestForBid(String pn, String sd, String mbp, String bti, String fn, HttpPostedFileBase image)
        {
            
           
            BidRequest bdr = new BidRequest();
            if (Request.Form["submit"] != null)
            {
                try
                {
                    bdr.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(bdr.ImageData, 0, image.ContentLength);
                    bdr.ProductName = pn;
                    bdr.Details = sd;
                    double bp = double.Parse(mbp);
                    bdr.BasePrice = bp;
 
                    int bt = Int32.Parse(bti);
                    bdr.MaxTime = bt;
                    bdr.FileName = fn;
                    string buyer = (string)(Session["log"]);
                    bdr.SellerName = buyer;
                    db.requests.Add(bdr);
                    db.SaveChanges();
                    ibc = 1;
                    
                    
                  
                }
                catch (DbEntityValidationException e)
                {
                    ViewBag.error1 = 1;
                    return View();
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), e);
                }
            }
            return RedirectToAction("Index");
            
        }
        public ActionResult AuctionAlert()
        {
            ViewBag.success = 0;
            return View();
        }
        [HttpPost]

        public ActionResult AuctionAlert(string HomeDecor, string Antiques, string Painting, string GovernmentProduct)
        {

            AuctionAlert  re1 = new AuctionAlert();
            ViewBag.success = 0;
            TempData["msg"] = "<script>alert('Auction Alert Created Successfully.');</script>";
            if (Request.Form["submit"] != null)
            {
                String user = (string)(Session["log"]);
                String a = "";
                var r = db.reg.Where(v => v.UserName.Equals(user));
                foreach (var item in r)
                {
                     a = item.Email;
                }


                if (HomeDecor != null)
                {
                    re1.Email = a;
                    re1.UserName = user;
                    re1.FavouriteCategory = HomeDecor;
                    db.alerts.Add(re1);
                    db.SaveChanges();
                    ViewBag.success = 1;
                    
                }
                if (Antiques != null)
                {
                    re1.Email = a;
                    re1.UserName = user;
                    re1.FavouriteCategory = Antiques;
                    db.alerts.Add(re1);
                    db.SaveChanges();
                    ViewBag.success = 1;
                }
                if (Painting != null)
                {
                    re1.Email = a;
                    re1.UserName = user;
                    re1.FavouriteCategory = Painting;
                    db.alerts.Add(re1);
                    db.SaveChanges();
                    ViewBag.success = 1;

                }

                if (GovernmentProduct != null)
                {
                    re1.Email = a;
                    re1.UserName = user;
                    re1.FavouriteCategory = GovernmentProduct;
                    db.alerts.Add(re1);
                    db.SaveChanges(); 
                    ViewBag.success = 1;
                }
            }

            return View();
        }
        public ActionResult logout()
        {
            Session["log"] = null;
            return RedirectToAction("Index");
           
        }




        //`````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(String fullName, String email, String subject, String message)
        {
            TempData["ContactSuccess"] = "Thanks " + fullName + ". Your message has been received and our team will contact you shortly.";
            return RedirectToAction("ContactUs");
        }

        public ActionResult OnlineHelp()
        {
            return View();
        }

        public ActionResult ChangeLocation()
        {
            return View();
        }

        public ActionResult OrderStatus()
        {
            return View();
        }

        public ActionResult Faqs()
        {
            return View();
        }
        //``````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->
        //``````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->
        public ActionResult TShirt()
        {
            return View();
        }

        public ActionResult Mens()
        {
            return View();
        }

        public ActionResult Womens()
        {
            return View();
        }

        public ActionResult GiftCards()
        {
            return View();
        }

        public ActionResult Shoes()
        {
            return View();
        }
        //```````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->    
        //``````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->
        public ActionResult TermsofUse()
        {
            return View();
        }

        public ActionResult PrivecyPolicy()
        {
            return View();
        }

        public ActionResult RefundPolicy()
        {
            return View();
        }

        public ActionResult BillingSystem()
        {
            return View();
        }

        public ActionResult TicketSystem()
        {
            return View();
        }
        //```````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->
        //``````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->
        public ActionResult CompanyInfo()
        {
            return View();
        }

        public ActionResult Careers()
        {
            return View();
        }

        public ActionResult StoreLocation()
        {
            return View();
        }

        public ActionResult AffillateProgram()
        {
            return View();
        }

        public ActionResult Copyright()
        {
            return View();
        }
        //```````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->
        //``````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->
        public ActionResult TradeAssurance()
        {
            return View();
        }

        public ActionResult BusinessIdentity()
        {
            return View();
        }

        public ActionResult LogisticsService()
        {
            return View();
        }

        public ActionResult SecurePayment()
        {
            return View();
        }

        public ActionResult InspectionService()
        {
            return View();
        }
        //```````````````````````````````````````````````````````````````````````````````````````````````````````````````````````-->






       
        public ActionResult Regis()
        {


            return View();
        }
        
        [HttpPost]
        
        public ActionResult Regis(String uname,String mail,String pword,String cpword,String cn,String ma)
        {
            Regi re = new Regi();
            
            if (Request.Form["submit"] != null)
            
            {
                try
                {
                    ViewBag.error = 0;
                    re.UserName = uname;
                    re.Email = mail;
                    re.Password = pword;
                    re.ConfirmPassword = cpword;
                    re.PhoneNumber = cn;
                    re.MailAddress = ma;
                    db.reg.Add(re);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    ViewBag.error = 1;
                    return View();
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), e);
                }
            }

            return RedirectToAction("login");
        }
       
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public object ProductName { get; set; }

        public object List { get; set; }
    }
}


