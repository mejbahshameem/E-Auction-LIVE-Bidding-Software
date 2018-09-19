
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class Regi
    {
        public int ID { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(50 ,ErrorMessage="Error")]
        [Display(Name = "User name")]
        public String UserName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public String Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public String Password { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact Number")]
        public String PhoneNumber { get; set; }
        [Display(Name = "Mailing Address")]
        public String MailAddress { get; set; }
       
    }
    public class Product
    {
        public int ID { get; set; }
        public String productID { get; set; }
        public String ProductName { get; set; }
        public String Category { get; set; }
        public String Details { get; set; }
        public String FileName { get; set; }
        public byte[] ImageData { get; set; }
        public String Status { get; set; }
        public int MaxTime { get; set; }
        public int BiddingTime { get; set; }
        public int CountClick { get; set; }
        public double BasePrice { get; set; }
        
      
        public double BiddingPrice { get; set; }
        public String BuyerName { get; set; }
        public String SellerName { get; set; }
        public String Date { get; set; }
        
    }

    public class Sold
    {
        public int ID { get; set; }
        public String productID { get; set; }
        public String FileName { get; set; }
        public byte[] ImageData { get; set; }
        public String BuyerName { get; set; }
        public String SellerName { get; set; }
        public double Price { get; set; }
        public String Date { get; set; }
     
    }

    public class BidRequest
    {
        public int ID { get; set; }
        public String SellerName { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public String ProductName { get; set; }
        [Required]
        [Display(Name = "Short Details")]
        public String Details { get; set; }
        
        public String FileName { get; set; }
        [Required]
        public byte[] ImageData { get; set; }
        [Required]
        [Display(Name = "Minimum Price of Your Product")]
        public double BasePrice { get; set; }
        [Required]
        [Display(Name = "Request Bid Time Interval")]
        public int MaxTime { get; set; }
        
    }
    public class AuctionAlert
    {
        public int ID { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public String FavouriteCategory{ get; set; }
    }
    public class Admin
    {
        public int ID { get; set; }
        [Required]
        public String UserName { get; set; }
        [Required]
        public String Password{ get; set; }
    }

    public class actionDbContext : DbContext
    {
        public DbSet<Regi> reg { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Sold> sales { get; set; }
        public DbSet<BidRequest> requests { get; set; }
        public DbSet<AuctionAlert> alerts { get; set; }
        public DbSet<Admin> admins { get; set; }


    }
}