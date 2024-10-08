using Microsoft.EntityFrameworkCore;
using ProblemSolvingTracker.DataManager;
using ProblemSolvingTracker.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyDbContext>(opt => opt.UseSqlite("Data Source=ProblemSolvingLog.db"));
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Frontend URL
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Include credentials if needed
        });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

app.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowFrontend");
app.MapGrpcService<TagService>().EnableGrpcWeb().RequireCors("AllowFrontend");

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();
