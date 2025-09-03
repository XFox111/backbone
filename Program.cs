using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyCompany("Eugene Fox")]
[assembly: AssemblyCopyright("© Eugene Fox 2025")]

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSignalR();

builder.Services.ConfigureHttpJsonOptions(options =>
	options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default)
);
builder.Services.Configure<JsonHubProtocolOptions>(options =>
	options.PayloadSerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default)
);

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
		[FromServices] IHubContext<WsHub> hubContext, [FromServices] ILogger<Program> logger,
		[FromQuery] string id, [FromBody] string data
	) =>
	{
		if (string.IsNullOrWhiteSpace(data) || data.Length > 66_560)
			return Results.BadRequest();

		logger.LogDebug("Received payload for connection '{id}' (package length: {len})", id, data.Length);
		await hubContext.Clients.Client(id).SendAsync("ReceiveData", data);

		return Results.Ok();
	}
);

app.Run();

class WsHub : Hub { }

[JsonSerializable(typeof(string))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
