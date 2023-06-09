using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using ProiectLicenta.Data;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Email;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;
using ProiectLicenta.Seed;
using ProiectLicenta.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

//Ef
builder.Services.AddDbContext<DataContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//Images
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

// Add services to the container.
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<ArtistRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<AlbumRepository>();
builder.Services.AddScoped<LocationRepository>();
builder.Services.AddScoped<GenreRepository>();
builder.Services.AddScoped<PlaylistRepository>();
builder.Services.AddScoped<SongRepository>();
builder.Services.AddScoped<TicketRepository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<SeedDatabase>();
builder.Services.AddScoped<IEmailSender,EmailSender>();
builder.Services.AddScoped<IUserService,UserService>();

//Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
        };
    });

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    
    builder.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader();
    /*
    builder.WithOrigins("https://remix-606e2.firebaseapp.com")
    .AllowAnyMethod()
    .AllowAnyHeader();
    */
}));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
app.UseCors("MyPolicy");

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var seed = services.GetRequiredService<SeedDatabase>();

    //Use the service
    if(seed != null)
        seed.SeedDb().Wait();

}
//images
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    RequestPath = new PathString("/Resources")
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
