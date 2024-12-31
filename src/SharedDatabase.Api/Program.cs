using SharedDatabase.Api.Endpoints;
using SharedDatabase.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseInfrastructure(app.Environment);

app
    .MapInfrastructure(app.Environment)
    .MapCompanyEndpoints()
    .MapSalesEndpoints();

app.Run();
