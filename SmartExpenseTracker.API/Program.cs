using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartExpenseTracker.Infrastructure;
using SmartExpenseTracker.Infrastructure.Data;
using SmartExpenseTracker.Infrastructure.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

// ✅ ADD SERVICES FIRST
//builder.Services.AddControllers();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("https://localhost") ||
                origin.StartsWith("http://localhost"))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(options => { options.SupportNonNullableReferenceTypes(); options.OperationFilter<SwaggerFileUploadOperationFilter>(); });
builder.Services.AddSwaggerGen(options => { options.OperationFilter<SwaggerFileUploadOperationFilter>(); });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure DI
//builder.Services.AddInfrastructure(builder.Configuration);

// ❗ NOW build the app
var app = builder.Build();

// ---------- MIDDLEWARE ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ UseCors AFTER Build
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();