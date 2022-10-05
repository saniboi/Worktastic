using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Worktastic.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
var serviceProvider = builder.Services.BuildServiceProvider();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
CreateRole(serviceProvider, "Admin").Wait();
CreateDefaulUser(serviceProvider, "Admin", "admin@worktastic.com", "Test1.").Wait();
app.Run();

async Task CreateRole(IServiceProvider serviceProvider, string roleName)
{
    var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
    var roleExists = await roleManager?.RoleExistsAsync(roleName)!;

    if (roleExists)
    {
        return;
    }

    await roleManager.CreateAsync(new IdentityRole(roleName));
}

async Task CreateDefaulUser(IServiceProvider serviceProvider, string roleName, string userName, string pw)
{
    var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

    var user = await userManager.FindByNameAsync(userName);

    if (user == null)
    {
        var newUser = new IdentityUser()
        {
            UserName = userName,
            Email = userName
        };

        await userManager.CreateAsync(newUser, pw);
    }

    user = await userManager.FindByNameAsync(userName);

    await userManager.AddToRoleAsync(user, roleName);
}