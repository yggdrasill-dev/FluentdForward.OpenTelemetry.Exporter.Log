using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging(logging =>
{
	logging.AddOpenTelemetry(otlOpt =>
	{
		otlOpt.AddFluentdForwardExporter(options =>
		{
			options.Tag = "test";
			options.UseMessagePack();
		});
	});
});

var app = builder.Build();

await app.StartAsync();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Test");

await app.WaitForShutdownAsync();
