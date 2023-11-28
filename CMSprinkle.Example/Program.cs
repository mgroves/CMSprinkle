using CMSprinkle.Auth;
using CMSprinkle.Couchbase;
using CMSprinkle.Example;
using CMSprinkle.Infrastructure;
using Couchbase.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCouchbase(options =>
{
    options.ConnectionString = "couchbase://localhost";
    options.UserName = "Administrator";
    options.Password = "password";
});

// this adds auth to CMSprinkle
// if don't do this, it will be local only
// ExampleAuthClass enables anonymous public access, so don't use it as-is!
builder.Services.AddTransient<ICMSprinkleAuth, ExampleAuthClass>();

// this adds CMSprinkle to your project
builder.Services.AddCMSprinkle(options =>
{
    // changes URL for cmsprinkle pages
    // if not specified, default is "cmsprinkle"
    // then URLs would be /cmsprinkle/home, etc
    options.RoutePrefix = "managecontent";

    // what message you want to show up when the
    // content hasn't been created yet
    // there is a default message if you don't specify this
    options.ContentNotFoundMessage = (contentKey) => $"ERROR: Can't find {contentKey}, did you add it yet?";
});

// this adds a Couchbase connection to CMSprinkle
await builder.Services.AddCMSprinkleCouchbaseAsync("Example","_default","_default", createCollectionIfNecessary: true);

// or here's the SQLServer provider
//await builder.Services.AddCMSprinkleSqlServerAsync("Server=localhost;Database=Example;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;", "SprinkleContent", "dbo",  createTableIfNecessary: true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
