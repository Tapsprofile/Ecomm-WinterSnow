using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WinterSnow.Data;
using WinterSnow.Services;
using WinterSnow.Services.Iam;
using WinterSnow.Services.System;
using WinterSnow.Services.Auditing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

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

// Basic audit log (Shopify parity foundation): record admin/vendor/customer API actions.
app.Use(async (ctx, next) =>
{
    await next();

    if (!ctx.Request.Path.StartsWithSegments("/api"))
        return;

    var audit = ctx.RequestServices.GetService<IAuditService>();
    if (audit is null)
        return;

    var userIdStr = ctx.User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? ctx.User.FindFirstValue("sub");
    _ = int.TryParse(userIdStr, out var userId);
    var role = ctx.User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);

    // Keep it lightweight; richer metadata can be added later.
    await audit.WriteAsync(new WinterSnow.Services.Auditing.AuditEntry
    {
        UserId = userId == 0 ? null : userId,
        Role = role,
        Action = $"{role ?? "Anonymous"}.{ctx.Request.Method}",
        Path = ctx.Request.Path,
        Method = ctx.Request.Method,
        StatusCode = ctx.Response.StatusCode
    });
});

app.UseAuthorization();
app.MapControllers();

app.Run();
