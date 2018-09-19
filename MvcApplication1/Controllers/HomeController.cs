using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
  
    public class HomeController : Controller
    {
        public static int ibc = 0;
        
        private actionDbContext db = new actionDbContext();
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
            return View();
        }
        public ActionResult SearchGVTA()
        {
            ViewBag.a = 1;
            ViewBag.c = 1;
            String a = "GovernmentProduct";
            var item = db.products.Where(v => v.Category.Equals(a));
            var list = item.ToList();
            if (list == null)
            {
                System.Diagnostics.Debug.WriteLine("hiiiiiiiiiiiiiiiii");
                ViewBag.c = 0;
               
            }
            return View(list);
        }
        public ActionResult SearchHD()
        {
            ViewBag.a = 1;
            String a = "HomeDecor";
            var item = db.products.Where(v => v.Category.Equals(a));
            var list = item.ToList();
            if (list == null)
            {
                ViewBag.b="No live Auction in your searched category.";
            }
            return View(list);
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
          
            return View(db.products.ToList());
        
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
                        string a = null;
                        var r = db.reg.Where(v => v.UserName.Equals(pr1buyer));
                        foreach (var item in r)
                        {
                            a = item.Email;
                        }

                        SmtpClient smtp = new SmtpClient(" smtp.gmail.com", 587);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 100000;

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;

                        smtp.Credentials = new NetworkCredential("eauction597@gmail.com", "eauction 597");
                        MailMessage message = new MailMessage();

                        message.To.Add(a);
                        message.From = new MailAddress("eauction597@gmail.com");
                        message.Subject = "LIVE BID WINNER!";
                        message.Body = "Your are the higest bidder for the product " + prd.ProductName + ". Please contact us ASAP for delivery & payment procedure.Thanks";
                        System.Diagnostics.Debug.WriteLine(a + message.Body);

                        smtp.Send(message);

                        string ab = null;
                        var r1 = db.reg.Where(v => v.UserName.Equals(soldproduct.SellerName));
                        foreach (var item in r)
                        {
                            ab = item.Email;
                        }


                        SmtpClient smtp1 = new SmtpClient(" smtp.gmail.com", 587);

                        smtp1.EnableSsl = true;
                        smtp1.Timeout = 100000;

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("eauction597@gmail.com", "eauction 597");
                        MailMessage message1 = new MailMessage();

                        message1.To.Add(ab);
                        message1.From = new MailAddress("eauction597@gmail.com");
                        message1.Subject = "LIVE BID PRODUCT SOLD!";
                        message1.Body = "Your product " + prd.ProductName + " has been sold for the price " + soldproduct.Price + ". Please contact us ASAP for further Information.Thanks";


                        smtp.Send(message1);

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
                        string a = null;
                        var r = db.reg.Where(v => v.UserName.Equals(pr1buyer));
                        foreach (var item in r)
                        {
                            a = item.Email;
                        }

                        SmtpClient smtp = new SmtpClient(" smtp.gmail.com", 587);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 100000;

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;

                        smtp.Credentials = new NetworkCredential("eauction597@gmail.com", "eauction 597");
                        MailMessage message = new MailMessage();

                        message.To.Add(a);
                        message.From = new MailAddress("eauction597@gmail.com");
                        message.Subject = "LIVE BID WINNER!";
                        message.Body = "Your are the higest bidder for the product " + prd.ProductName + ". Please contact us ASAP for delivery & payment procedure.Thanks";
                        System.Diagnostics.Debug.WriteLine(a + message.Body);

                        smtp.Send(message);

                        string ab = null;
                        var r1 = db.reg.Where(v => v.UserName.Equals(soldproduct.SellerName));
                        foreach (var item in r)
                        {
                            ab = item.Email;
                        }


                        SmtpClient smtp1 = new SmtpClient(" smtp.gmail.com", 587);

                        smtp1.EnableSsl = true;
                        smtp1.Timeout = 100000;

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("eauction597@gmail.com", "eauction 597");
                        MailMessage message1 = new MailMessage();

                        message1.To.Add(ab);
                        message1.From = new MailAddress("eauction597@gmail.com");
                        message1.Subject = "LIVE BID PRODUCT SOLD!";
                        message1.Body = "Your product " + prd.ProductName + " has been sold for the price " + soldproduct.Price + ". Please contact us ASAP for further Information.Thanks";


                        smtp.Send(message1);

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
                        string a = null;
                        var r = db.reg.Where(v => v.UserName.Equals(pr1buyer));
                        foreach (var item in r)
                        {
                            a = item.Email;
                        }

                        SmtpClient smtp = new SmtpClient(" smtp.gmail.com", 587);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 100000;

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;

                        smtp.Credentials = new NetworkCredential("eauction597@gmail.com", "eauction 597");
                        MailMessage message = new MailMessage();

                        message.To.Add(a);
                        message.From = new MailAddress("eauction597@gmail.com");
                        message.Subject = "LIVE BID WINNER!";
                        message.Body = "Your are the higest bidder for the product " + prd.ProductName + ". Please contact us ASAP for delivery & payment procedure.Thanks";
                        System.Diagnostics.Debug.WriteLine(a + message.Body);

                        smtp.Send(message);

                        string ab = null;
                        var r1 = db.reg.Where(v => v.UserName.Equals(soldproduct.SellerName));
                        foreach (var item in r)
                        {
                            ab = item.Email;
                        }


                        SmtpClient smtp1 = new SmtpClient(" smtp.gmail.com", 587);

                        smtp1.EnableSsl = true;
                        smtp1.Timeout = 100000;

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("eauction597@gmail.com", "eauction 597");
                        MailMessage message1 = new MailMessage();

                        message1.To.Add(ab);
                        message1.From = new MailAddress("eauction597@gmail.com");
                        message1.Subject = "LIVE BID PRODUCT SOLD!";
                        message1.Body = "Your product " + prd.ProductName + " has been sold for the price " + soldproduct.Price + ". Please contact us ASAP for further Information.Thanks";


                        smtp.Send(message1);

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
