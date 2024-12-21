using SharedDatabase.Endpoints;
using SharedDatabase.Infrastructure;
using SharedDatabase.Infrastructure.Common.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<MultiTenancyOperationFilter>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseInfrastructure();

app.MapCompanyEndpoints()
    .MapSalesEndpoints();

app.Run();
