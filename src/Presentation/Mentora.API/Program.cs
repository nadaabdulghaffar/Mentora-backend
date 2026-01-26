using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mentora.Application.Interfaces;
using Mentora.Application.Interfaces.Repositories;
using Mentora.Application.Services;
using Mentora.Application.Validators;
using Mentora.Infrastructure.Configuration;
using Mentora.Infrastructure.Services;
using Mentora.Persistence;
using Mentora.Persistence.Repositories;
using System.Text;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories and Unit of Work
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMenteeProfileRepository, MenteeProfileRepository>();
builder.Services.AddScoped<IMentorProfileRepository, MentorProfileRepository>();
builder.Services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

// Add Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();


// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterInitialRequestValidator>();

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MenteeOnly", policy => policy.RequireRole("Mentee"));
    options.AddPolicy("MentorOnly", policy => policy.RequireRole("Mentor"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(builder.Configuration["CorsOrigins"]?.Split(',') ?? new[] { "http://localhost:3000" })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure static files for serving uploaded files
builder.Services.AddDirectoryBrowser();


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Mentorship Platform API";
        options.Theme = ScalarTheme.Moon;
    });
}

// Enable static files (for serving uploaded files)
app.UseStaticFiles();

app.MapControllers();
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

// Ensure database is created (for development)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    
    // Create uploads folder
    var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
    if (!Directory.Exists(webRoot))
    {
        Directory.CreateDirectory(webRoot);
    }
    
    var uploadsPath = Path.Combine(webRoot, "uploads");
    Directory.CreateDirectory(Path.Combine(uploadsPath, "cvs"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "profile-pictures"));
}

app.Run();