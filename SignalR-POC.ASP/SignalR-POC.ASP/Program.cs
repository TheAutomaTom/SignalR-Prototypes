using SignalR_POC.ASP.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
			builder =>
			{
				builder
				.WithOrigins(
					// SignalR-POC.Vue3:
					"http://localhost:5173"

					// SignalR-POC.Electron-Vue3:
					// Electron does not require a listing, while debugging locally.
					// Will that be still true on a server?
					)
					.AllowAnyHeader()
					.WithMethods("GET", "POST")
					.AllowCredentials();
			});
});
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
	var forecast = Enumerable.Range(1, 5).Select(index =>
			new WeatherForecast
			(
					DateTime.Now.AddDays(index),
					Random.Shared.Next(-20, 55),
					summaries[Random.Shared.Next(summaries.Length)]
			))
			.ToArray();
	return forecast;
});

// UseCors must be called before MapHub.
app.UseCors();

app.MapHub<ChatHub>("/chatHub");

app.Run();







internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}