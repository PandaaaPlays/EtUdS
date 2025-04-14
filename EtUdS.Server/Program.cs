using EtUdS.Server;
using etuds.Server.Data;
using EtUdS.Server.Repositories;
using EtUdS.Server.Repositories.Equipe;
using etuds.Server.Repositories.Etudiant;
using EtUdS.Server.Repositories.Membre;
using etuds.Server.Services.Conversation;
using etuds.Server.Services.Cours;
using etuds.Server.Services.Devoir;
using etuds.Server.Services.Document;
using etuds.Server.Services.Etudiant;
using Microsoft.AspNetCore.Authentication.Cookies;
using etuds.Server.Services.Equipe;
using etuds.Server.Services.Membre;
using etuds.Server.Services.NoteDeCours;
using etuds.Server.Services.Message;
using EtUdS.Server.Services.Message;
using etuds.Server.Services.Professeur;
using etuds.Server.Services.Seance;
using etuds.Server.Services.Soumission;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins!) 
            .AllowAnyHeader() // Allows any header
            .AllowAnyMethod() // Allows any HTTP method (GET, POST, etc.)
            .AllowCredentials()
            .WithExposedHeaders("X-File-Name");
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<ICoursService, CoursService>();
builder.Services.AddScoped<IEtudiantRepository, EtudiantRepository>();
builder.Services.AddScoped<IEtudiantService, EtudiantService>();
builder.Services.AddScoped<IDevoirService, DevoirService>();
builder.Services.AddScoped<ISeanceService, SeanceService>();
builder.Services.AddScoped<IMembreService, MembreService>();
builder.Services.AddScoped<IEquipeService, EquipeService>();
builder.Services.AddScoped<IEquipeRepository, EquipeRepository>();
builder.Services.AddScoped<IMembreRepository, MembreRepository>();
builder.Services.AddScoped<ISoumissionService, SoumissionService>();
builder.Services.AddScoped<IProfesseurService, ProfesseurService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<INoteDeCoursService, NoteDeCoursService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "EtUdSCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None; 
        options.LoginPath = "/api/Connexion";
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<RappelService>();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
