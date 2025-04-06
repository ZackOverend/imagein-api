var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddAuthorization(); // ✅ This is the missing line
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://imagein-zackaryoverend.vercel.app/", "https://imagein-gamma.vercel.app/", "https://imagein-zacks-projects-771ac82d.vercel.app/")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization(); // ✅ Now this works

app.MapControllers();

app.Run();