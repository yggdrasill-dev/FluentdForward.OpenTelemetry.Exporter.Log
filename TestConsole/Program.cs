using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging
	.AddOpenTelemetry(otlOpt =>
	{
		otlOpt.IncludeFormattedMessage = true;
		_ = otlOpt.AddFluentdForwardExporter(options =>
		  {
			  options.Host = "192.168.2.12";
			  options.Tag = "ttt";
			  options.UseMessagePack();
		  });
	});

var app = builder.Build();

await app.StartAsync();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Test");

await app.WaitForShutdownAsync();
