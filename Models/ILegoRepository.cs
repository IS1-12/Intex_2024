﻿namespace LegosWithAurora.Models
{
    public interface ILegoRepository
    {
        IQueryable<Order> Orders { get; }
        IQueryable<Product> Products { get; }
        IQueryable<LineItem> LineItems { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<AspNetUser> AspNetUsers {get;}
        IQueryable<AspNetRole> AspNetRoles {get;}
        IQueryable<AspNetRoleClaim> AspNetRoleClaims {get;}
        IQueryable<AspNetUserLogin> AspNetUserLogins {get;}
        IQueryable<AspNetUserToken> AspNetUserTokens {get;}
        IQueryable<Recommendations> recommendations { get;}
        public void AddProduct(Product p);
        public Product RemoveProduct(int id);
        public void RemoveProduct(Product p);
        public Product EditProduct(int id);
        public void EditProduct(Product p);

        public Order RejectOrder(int id);

        public void RejectOrder(Order o);

        public void CorrectOrder(int id);
        //public AspNetUser UpdateUser(int id);
        
        public void AddOrder(Order o);
        public void AddLineItem(LineItem l);
    }
}
