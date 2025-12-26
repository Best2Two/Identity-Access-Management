//Loading Environment
using DotNetEnv;
using IAMService.Common.Config;
using IAMService.Services;

var builder = WebApplication.CreateBuilder(args);

//Add environment variables
Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.PostConfigure<SecuritySettings>(settings =>
{
    // This runs strictly inside the DI container. No manual "new" keyword.
    settings.PrivateKey = settings.PrivateKey.Replace("\\n", "\n");
    settings.PublicKey = settings.PublicKey.Replace("\\n", "\n");
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
