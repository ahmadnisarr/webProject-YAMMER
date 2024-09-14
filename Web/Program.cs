using Microsoft.EntityFrameworkCore;
using WebProject.Data;
using Domain_Core;
using Application;
using Infrastructure;
using WebProject.Models;
using Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<userNewField>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//Add services of DI in Project

builder.Services.AddSingleton<IPostingService,PostingService>();
builder.Services.AddSingleton<IUserProfileService,UserProfileService>();
builder.Services.AddSingleton<IRepositoryService<Posting>,RepositoryService<Posting>>();
builder.Services.AddSingleton<IRepositoryService<images>,RepositoryService<images>>();
builder.Services.AddSingleton<IRepositoryService<postANDimage>,RepositoryService<postANDimage>>();
builder.Services.AddSingleton<IRepositoryService<userProfile>,RepositoryService<userProfile>>();

builder.Services.AddSingleton<IRepository<postANDimage>>(new  GenericRepository<postANDimage>(connectionString));
builder.Services.AddSingleton<IRepository<images>>(new GenericRepository<images>(connectionString));
builder.Services.AddSingleton<IRepository<Posting>>(new GenericRepository<Posting>(connectionString));
builder.Services.AddSingleton<IRepository<userProfile>>(new GenericRepository<userProfile>(connectionString));
builder.Services.AddSingleton<IPosting>(provider =>
{
    var postingRepository = new PostingRepository(connectionString,
        provider.GetRequiredService<IRepository<Posting>>(),
        provider.GetRequiredService<IRepository<images>>());

    return postingRepository;
});
builder.Services.AddSingleton<IUserProfile>(prov =>
{ 
            var  userProfileRepository = new userProfileRepository(connectionString,
                 prov.GetRequiredService<IRepository<userProfile>>());
return userProfileRepository;
    
});



builder.Services.AddAuthorization(option=>
 option.AddPolicy("after6Messaging", policy =>
 {
     policy.RequireAssertion(context=> DateTime.Now.Hour<18 &&  DateTime.Now.Hour > 6);
     
 }));

builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseResponseCaching();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<chatHub>("/chatHub");
}) ;


app.Run();
