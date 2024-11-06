using api_sk1_02files.Controllers;
using api_sk1_02files.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddOptions();
builder.Services.Configure<ThumbnailOptions>(builder.Configuration.GetSection("Thumbnails"));
builder.Services.Configure<PictureOptions>(builder.Configuration.GetSection("Pictures"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowSomeOrigin", policy => policy.WithOrigins("http://something.test"));
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
//app.UseCors("AllowSomeOrigin");
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
