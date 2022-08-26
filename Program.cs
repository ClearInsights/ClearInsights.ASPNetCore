using ClearInsights.Logging;
using ClearInsights.Monitoring;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add to capture logs with ClearInsights Logging product
builder.Logging.AddClearInsightsLogger(configuration =>
{
    configuration.ApiKey = "{ApiKey}";
    configuration.Secret = "{Environment Client Secret}";
    configuration.ApplicationName = "{Application Name}";
});

// Add to capture performance metrics with the Monitoring product
builder.Services.AddClearInsightsMonitor(configuration =>
{
    configuration.ApiKey = "{ApiKey}";
    configuration.Secret = "{Environment Client Secret}";
    configuration.ApplicationName = "{Application Name}";
});

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

//Add to use ClearInsights global error handling.
//This will automatically catch any unhandled exceptions
app.UseClearInsightsExceptionHandling(options =>
{
    //Add to extend the error handler and add additional logic.
    //Add logic like custom HTTP response, etc...
    options.OnError += (sender, arg) =>
    {
        var response = "Oops something went wrong";
        arg.HttpContext.Response.ContentType = "text/html";
        arg.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        arg.HttpContext.Response.WriteAsync(response);
    };
});

app.Run();
