using System.Text;
using AI_Scan_Analyses_Test.ChatBot.Client;
using AI_Scan_Analyses_Test.Extintions;
using DomainLayer.Interfaces;
using DomainLayer.models.AuthModles;
using DomainLayer.models.ModelsHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer;
using RepositoryLayer.Data;

using ServicesLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DataBaseConnections
// Add DbContext with Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 
#endregion

#region Auth_Identity

// Add Identity and JWT Authentication using the extension method
builder.Services.AddIdentityServices(builder.Configuration);

// Add Authorization
builder.Services.AddAuthorization();
#endregion

#region Services_ChatBot
//Service OF ChatBot
string apiKey = "AIzaSyD5X3RS-GWfsDowTQ84N4h4eyRZFAATzoE";
builder.Services.AddSingleton(new GeminiApiClient(apiKey));
#endregion

#region HttpClient Configuration
/// Register HttpClient with configuration
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient("FlaskApiClient", client =>
{
    var apiSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.FlaskApiBaseUrl);
});

#endregion

// Register ImageSettings
builder.Services.Configure<ImageSettings>(builder.Configuration.GetSection("ImageSettings"));

#region Services_DJ
// Register repositories and services
builder.Services.AddScoped<IScanAnalysisService, ScanAnalysisService>();
builder.Services.AddScoped<IScanAnalysisRepository, ScanAnalysisRepository>();
builder.Services.AddScoped<IStudyRepository, StudyRepository>();
builder.Services.AddScoped<IImageResolverService, ImageResolverService>();
builder.Services.AddScoped<IAuthService, AuthService>();

#endregion

var app = builder.Build();
#region Chat_Bot

//ApplayChatBot
app.UseCors(option => option
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Dispostion")); 
#endregion

#region UpdateDataBase

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var appDbContext = services.GetRequiredService<ApplicationDbContext>();
        appDbContext.Database.Migrate(); // Apply migrations for ApplicationDbContext

        //var identityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();
        //identityDbContext.Database.Migrate(); // Apply migrations for ApplicationIdentityDbContext
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations.");
    }
}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
    app.UseSwagger();
    app.UseSwaggerUI();
// Serve static files from wwwroot
app.UseStaticFiles();
app.UseHttpsRedirection();

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();