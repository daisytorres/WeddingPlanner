using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //added this


builder.Services.AddHttpContextAccessor(); //added this
builder.Services.AddSession(); //added this
builder.Services.AddDbContext<MyContext>(options => //added this
{
   options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession(); //added this


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
