using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProiectLicenta.Data;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Email;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;
using ProiectLicenta.Seed;
using ProiectLicenta.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<ArtistRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<AlbumRepository>();
builder.Services.AddScoped<LocationRepository>();
builder.Services.AddScoped<PlaylistRepository>();
builder.Services.AddScoped<SongRepository>();
builder.Services.AddScoped<TicketRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<SeedDatabase>();
builder.Services.AddScoped<IEmailSender,EmailSender>();
builder.Services.AddScoped<IUserService,UserService>();

//Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/user/Login"; // Pagina unde se face redirect la autentificare
    });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
 });

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var seed = services.GetRequiredService<SeedDatabase>();

    //Use the service
    if(seed != null)
        seed.SeedDb().Wait();

}


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
