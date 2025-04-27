using Microsoft.EntityFrameworkCore;

namespace StationeryShop.Models
{
    public class StationeryDbContext : DbContext
    {
        public StationeryDbContext(DbContextOptions<StationeryDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Client>().ToTable("Clients");
            modelBuilder.Entity<Cart>().ToTable("Carts");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Review>().ToTable("Reviews");

            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Cart>().HasKey(c => c.Id);
            modelBuilder.Entity<Client>().HasKey(c => c.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Review>().HasKey(r => r.Id);


            //1-to-1: Client <-> Cart
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Cart)
                .WithOne(c => c.Client)
                .HasForeignKey<Cart>(c => c.ClientId);

            //1-to-many: Client -> Orders
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Client)
                .HasForeignKey(o => o.ClientId);

            //1-to-many: Client -> Reviews
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Reviews)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.ClientId);

            //1-to-many: Product -> CartItems
            modelBuilder.Entity<Product>()
                .HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId);

            //1-to-many: Product -> OrderItems
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId);

            //1-to-many: Product -> Reviews
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId);

            //1-to-many: Cart -> CartItems
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);

            //1-to-many: Order -> OrderItems
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            //индексы для ускорения поиска
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name);

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasIndex(r => r.ProductId);

            //значения по умолчанию
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}