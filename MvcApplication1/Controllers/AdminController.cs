using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class AdminController : Controller
    {
        public static int postreqid=0;
        public static int success = 0;
        private actionDbContext db = new actionDbContext();


        //
        // GET: /Admin/
       public ActionResult Repost(int id)
        {
            success = 0;
            int count = db.products.Count();
            Product product1=db.products.Find(id); 
            Product product=product1;
            db.products.Remove(product1);
            db.SaveChanges();
             if (count < 13)
                   
                    {
               
                        db.products.Add(product);
                        db.SaveChanges();
                        success = 1;
                        string a = null ;
                        var r = db.reg.Where(v => v.UserName.Equals(product.SellerName));
                        foreach (var item in r)
                        {
                         a=item.Email;
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
                        message.Subject = "LIVE BID!";
                        message.Body = "Your product now in Live Auction. Please See Home of E-AUCTION for further confirmation.";


                        smtp.Send(message);
                        return RedirectToAction("Table");
                    }
                
                else
                {
                    success = 2;
                   
                    return RedirectToAction("Table");
                }
                
            

         
    } 

        public ActionResult DeleteLive(int id)
        {
            Product aa = db.products.Find(id);
            if (aa != null)
            {
                db.products.Remove(aa);
                db.SaveChanges();

            }

            return RedirectToAction("LiveAuction");
        }
        public ActionResult DetailsLive(int id)
        {
            Product rg = db.products.Find(id);
            if (rg != null)
            {
                return View();

            }

            return RedirectToAction("LiveAuction");
        }
        public ActionResult UserInfo()
        {

            return View(db.reg.ToList());
        }
        public ActionResult LiveAuction()
        {

            return View(db.products.ToList());
        }

        public ActionResult AuctionAlert1()
        {

            return View(db.alerts.ToList());
        }
        public ActionResult SoldProduct()
        {

            return View(db.sales.ToList());
        }

        public ActionResult DeleteAlert(int id)
        {
            AuctionAlert aa = db.alerts.Find(id);
            if (aa != null)
            {
                db.alerts.Remove(aa);
                db.SaveChanges();

            }

            return RedirectToAction("AuctionAlert1");
        }
        public ActionResult DetailsSoldProduct(int id)
        {
            Sold rg = db.sales.Find(id);
            if (rg != null)
            {
                return View();

            }

            return RedirectToAction("Table");
        }
        public ActionResult UserInfo1(int id)
        {
            Regi rg = db.reg.Find(id);
            if (rg != null)
            {
                db.reg.Remove(rg);
                db.SaveChanges();

            }

            return RedirectToAction("UserInfo");
        }
        public ActionResult Table()
        {
            if (success == 1)
            {
                ViewBag.success = 1;
               
                TempData["msg"] = "<script>alert('Add to live Auction Successful.Please delete entry from bid request.');</script>";
                success = 0;

            }
            else if (success == 2)
            {
                ViewBag.success = 2;
                TempData["msg"] = "<script>alert('Too many live auction in Process.Try again later.');</script>";
                success = 0;
            }
            else
            {
                ViewBag.success = 0;
            }
            return View();
        }
        public ActionResult Deletereq(int id)
        {
            BidRequest bdr = db.requests.Find(id);
            if (bdr!=null)
            {
                db.requests.Remove(bdr);
                db.SaveChanges();

            }
           
            return RedirectToAction("Table");
        }
        public ActionResult BidRequest()
        {
            return View(db.requests.ToList());
        }
        public ActionResult postreq(int id)
        {
            postreqid = id;
            return View();
        }
        [HttpPost]
        public ActionResult postreq(Product product)
        {
            success = 0;
            int count = db.products.Count();
            BidRequest bdr = db.requests.Find(postreqid);
            if (ModelState.IsValid)
            {
                if (count < 13)
                {
                    if (bdr != null)
                    {
                        product.productID = bdr.ProductName + postreqid;
                        product.ProductName = bdr.ProductName;
                        product.Details = bdr.Details;
                        product.FileName = bdr.FileName;
                        product.ImageData = bdr.ImageData;
                        product.BasePrice = bdr.BasePrice;
                        product.BiddingPrice = bdr.BasePrice;
                        product.SellerName = bdr.SellerName;
                        db.products.Add(product);
                        db.SaveChanges();
                        success = 1;
                        
                        var r1 = db.alerts.Where(v => v.FavouriteCategory.Equals(product.Category));
                        foreach (var item1 in r1)
                        {
                            SmtpClient smtp1 = new SmtpClient(" smtp.gmail.com", 587);

                            smtp1.EnableSsl = true;
                            smtp1.Timeout = 100000;

                            smtp1.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp1.UseDefaultCredentials = false;

                            smtp1.Credentials = new NetworkCredential("eauction597@gmail.com", "eauction 597");
                            MailMessage message1 = new MailMessage();

                            message1.To.Add(item1.Email);
                            message1.From = new MailAddress("eauction597@gmail.com");
                            message1.Subject = "LIVE BID!";
                            message1.Body = "NEW product in"+item1.FavouriteCategory+" Section. Please Visit our website! ";


                            smtp1.Send(message1);

                        }
                        string a = null;
                        var r = db.reg.Where(v => v.UserName.Equals(bdr.SellerName));
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
                        message.Subject = "LIVE BID!";
                        message.Body = "Your product now in Live Auction. Please See Home of E-AUCTION for further confirmation.";


                        smtp.Send(message);
                        return RedirectToAction("Table");
                    }
                }
                else
                {
                    success = 2;
                   
                    return RedirectToAction("Table");
                }
                
            }

            return View(product);
           
           
        }
        public ActionResult logout()
        {
            Session["log1"] = null;
            return RedirectToAction("Index");

        }
       
        public ActionResult Deletereg()
        {
            Regi re = db.reg.Find();
            if (re == null)
            {
                return HttpNotFound();
            }
            return View(re);
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(String un,String pd)
        {
            int i = 0;
            ViewBag.error2 = 0;

            if (Request.Form["submit1"] != null)
            {

                List<Admin> item1 = db.admins.ToList();
                foreach (var d in item1)
                {
                    if (d.UserName.Equals(un) && d.Password.Equals(pd))
                    {
                        i = 1;
                        Session["log1"] = un;
                        string buyer = (string)(Session["log1"]);
                        System.Diagnostics.Debug.WriteLine(buyer);
                        return RedirectToAction("Table");
                    }

                }
                if (i == 0)
                {
                    ViewBag.error2 = 1;
                    return View();
                }
                    
                
            }
            return View();
                  
        }
        public ActionResult AddProduct()
        {
            Product p = new Product();


            return View(p);
        }
        [HttpPost]
        public ActionResult AddProduct(Product model, HttpPostedFileBase image1)
        {
            

            if (image1 != null)
            {
                model.ImageData = new byte[image1.ContentLength];
                image1.InputStream.Read(model.ImageData, 0, image1.ContentLength);
            }
            db.products.Add(model);
            db.SaveChanges();
            return View(model);
        }

    }
}
