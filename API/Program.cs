using System.Text;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SignalIR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(opt=>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    opt.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    //   .EnableDetailedErrors();
});

builder.Services.AddCors();

builder.Services.AddScoped<ITokenSerervice, TokenService>();

builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

builder.Services.AddScoped<LogUserActivity>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings")); 
//signalIr 
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();

builder.Services.AddIdentityCore<AppUser>(opt =>
{

    opt.Stores.SchemaVersion = IdentitySchemaVersions.Version2;
        opt.SignIn.RequireConfirmedAccount = false;
   
   opt.Password.RequireNonAlphanumeric =false;
   opt.User.RequireUniqueEmail= true;
   

}).AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();





builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var tokenKey = builder.Configuration["TokenKey"]?? throw new Exception("Token key not found - Program.cs");

options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
    ValidateIssuer =false,
    ValidateAudience= false
};


//signalIr
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if(!string.IsNullOrEmpty(accessToken)&& path.StartsWithSegments("/hubs"))
            {
                
                context.Token = accessToken;

            }

            return Task.CompletedTask;

        }

    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))

    .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin","Moderator"));


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x=>x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
.WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//signalIr
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/messages");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try{

var context = services.GetRequiredService<AppDbContext>();
var userManager = services.GetRequiredService<UserManager<AppUser>>();
await context.Database.MigrateAsync();
await context.Connections.ExecuteDeleteAsync();
await Seed.SeedUsers(userManager);

}

catch(Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
    

}

app.Run();