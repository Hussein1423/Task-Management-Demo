using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Task_Management_API.Entities;
using Task_Management_API.Messaging;
using Task_Management_API.Middlewares;
using Task_Management_API.Services;
using DotNetEnv;

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    Env.Load();
}



var builder = WebApplication.CreateBuilder(args);

////var name = builder.Configuration["AppSettings:ApplicationName"];
////var maxTasks = builder.Configuration["AppSettings:MaxTasks"];


builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.AddProblemDetails();

builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddControllers();
builder.Services.AddScoped<ITaskService, TaskService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection" )
    )
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();