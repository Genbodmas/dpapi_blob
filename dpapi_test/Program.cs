using dpapi_test.models;
using dpapi_test.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeyStoreOptions>(builder.Configuration.GetSection("KeyStore"));
builder.Services.AddSingleton<IKeyStore, DpapiKeyStore>();

builder.Services.AddControllers();

// swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "dpapi_test v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
