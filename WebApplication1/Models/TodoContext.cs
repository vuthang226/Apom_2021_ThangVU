using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoContext : DbContext
    {
        #region DECLARE
        //protected string _connectionString = "Host=localhost;User Id=root; password=;Database=cukcuk_demo;port=3306;Character Set=utf8";
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }
        #endregion

        public DbSet<TodoItem> TodoItems { get; set; }
        
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_connectionString);
        //}
  


    }
}
