using CepMinimalAPI.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/cep/{cep}", async (string cep, IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor) =>
{
	var client = httpClientFactory.CreateClient();
	var response = await client.GetStringAsync($"https://viacep.com.br/ws/{cep}/json/");

	if (response.Contains("erro"))
	{
		return Results.NotFound("CEP not found");
	}

	CepBasicResponse? address = null;

	if (contextAccessor.HttpContext != null && contextAccessor.HttpContext.User != null)
	{
		var customClaim = contextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "extension_CEPQueryProfile");
		if (customClaim?.Value == "basic")
		{
			address = JsonConvert.DeserializeObject<CepBasicResponse>(response);
		}
		else
		{
			address = JsonConvert.DeserializeObject<CepResponse>(response);
		}
	}


	return Results.Json(address);
})
.WithOpenApi()
.RequireAuthorization();

app.Run();
