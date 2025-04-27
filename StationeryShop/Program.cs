using Microsoft.EntityFrameworkCore;
using StationeryShop.Models;
using StationeryShop.Services;
using StationeryShop.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7038")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errorMessage = context.ModelState
                .SelectMany(m => m.Value.Errors)
                .FirstOrDefault()?.ErrorMessage ?? "Неверные данные";

            return new BadRequestObjectResult(new { error = errorMessage });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<StationeryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<SessionCartService>();


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Stationery.Session";
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Stationery.Auth";
        options.LoginPath = "/api/Auth/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("ApiPolicy");

app.UseAuthentication();

app.UseSession();

app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Request.Path.StartsWithSegments("/api")
        && context.HttpContext.Response.StatusCode == 401)
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            error = "Требуется авторизация",
            solution = "Пожалуйста, войдите через /api/Auth/login",
            status = 401
        }));
    }
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<StationeryDbContext>();

        context.Database.Migrate();

        if (context.Products.Count() == 0)
        {
            SeedData.Initialize(context);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();
