using Finbuckle.MultiTenant;
using SharedDatabase.Api.Endpoints;
using SharedDatabase.Infrastructure;
using SharedDatabase.Infrastructure.Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebServices()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1/openapi.json");
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1/openapi.json", "Example API");
        options.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseMultiTenant();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notifications");

app.MapCompanyEndpoints()
    .MapSalesEndpoints();

app.Run();
