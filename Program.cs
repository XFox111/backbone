
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyCompany("Eugene Fox")]
[assembly: AssemblyCopyright("© Eugene Fox 2025")]

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
		policy
			.SetIsOriginAllowed(_ => true)
			.WithMethods(["POST"])
			.WithHeaders(["x-requested-with", "x-signalr-user-agent"])
			.AllowCredentials());
});

WebApplication app = builder.Build();

app.UseCors();

app.MapHub<WsHub>("/ws", options =>
{
	options.ApplicationMaxBufferSize = 1;       // 1B, virtually disabling ability for clients to send data
});

app.MapPost("/send",
	static async (
		[FromServices] IHubContext<WsHub> hubContext,
		[FromServices] ILogger<Program> logger,
		HttpContext context
	) =>
	{
		string? id = context.Request.Query["id"].FirstOrDefault();

		// Id validation
		if (string.IsNullOrEmpty(id) || id.Length > 32)
			return Results.StatusCode(StatusCodes.Status400BadRequest);

		foreach (char c in id)
			if (!char.IsAsciiLetterOrDigit(c) && c is not ('-' or '_'))
				return Results.StatusCode(StatusCodes.Status400BadRequest);

		// Content validation
		if (context.Request.ContentType is not "text/plain")
			return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);

		if (context.Request.ContentLength is < 1 or > 66_560)
			return Results.StatusCode(StatusCodes.Status400BadRequest);

		// Sending data
		if (logger.IsEnabled(LogLevel.Debug))
			logger.LogDebug("Received payload for connection '{id}' (package length: {len} B)", id, context.Request.ContentLength);

		using StreamReader reader = new(context.Request.Body);
		string? data = await reader.ReadToEndAsync();

		await hubContext.Clients.Client(id).SendAsync("OnMessage", data);

		return Results.Ok();
	}
);

app.Run();

class WsHub : Hub { }
