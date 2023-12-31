﻿using EBook.DataAccess.Data;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext context;
        public UnitOfWork(ApplicationDbContext _context)
        {
            context = _context;
            Category = new CategoryRepository(context);
            CoverType = new CoverTypeRepository(context);
            Product = new ProductRepository(context);
            Company = new CompanyRepository(context);
            ApplicationUser= new ApplicationUserRepository(context);
            ShoppingCart =new ShoppingCartRepository(context);
            Order = new OrderRepository(context);
            OrderDetails= new OrderDetailsRepository(context);
        }
        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; set; }
        public IShoppingCartRepository ShoppingCart { get; set; }
        public IOrderRepository Order { get; set; }
        public IOrderDetailsRepository OrderDetails { get; set; }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
