using System.Threading.RateLimiting;
using InterServer.Logic;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Thing to make JSON case-sensitive, unused for now. Breaks some things
// builder.Services.AddControllers()
// 	.AddJsonOptions(options =>
// 	{
// 		options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
// 		options.JsonSerializerOptions.PropertyNamingPolicy = null;             
// 	});

// Add services to the container.
builder.Services.AddControllersWithViews();

// API versioning and such
builder.Services.AddApiVersioning(opt =>
{
	opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);
	opt.AssumeDefaultVersionWhenUnspecified = true;
	opt.ReportApiVersions = true;
	opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
		new HeaderApiVersionReader("x-api-version"),
		new MediaTypeApiVersionReader("x-api-version"));
});

builder.Services.AddRateLimiter(_ => _
	.AddFixedWindowLimiter(policyName: "fixed", options =>
	{
		options.PermitLimit = 10;
		options.Window = TimeSpan.FromSeconds(12);
		options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		options.QueueLimit = 5;
	}));

// Background timed data collector
builder.Services.AddSingleton<DataCollector>();
builder.Services.AddHostedService<DataCollector>();

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

app.UseRateLimiter();

app.Run();