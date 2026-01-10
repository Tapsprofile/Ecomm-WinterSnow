using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WinterSnow.Data;
using WinterSnow.Services;
using WinterSnow.Services.Iam;
using WinterSnow.Services.System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddPolicy("frontend", p =>
    {
        p.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<WinterSnowDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("WinterSnow") ?? "Data Source=wintersnow.dev.db";
    options.UseSqlite(conn);
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddWinterSnowServices();
builder.Services.AddScoped<DatabaseBootstrapper>();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed demo data (dev-friendly).
using (var scope = app.Services.CreateScope())
{
    if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Dev:ResetDbOnStartup"))
    {
        var db = scope.ServiceProvider.GetRequiredService<WinterSnowDbContext>();
        await db.Database.EnsureDeletedAsync();
    }
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseBootstrapper>();
    await seeder.EnsureSeededAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");

// Metrics + error capture for "System Health" panel.
app.Use(async (ctx, next) =>
{
    var metrics = ctx.RequestServices.GetRequiredService<ApiMetrics>();
    var sw = System.Diagnostics.Stopwatch.StartNew();
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        metrics.RecordError(ex);
        throw;
    }
    finally
    {
        sw.Stop();
        metrics.RecordRequest(sw.Elapsed);
    }
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
