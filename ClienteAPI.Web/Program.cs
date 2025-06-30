using ClienteAPI.Data.Contexts;
using ClienteAPI.Data.IdentityContexts;
using ClienteAPI.Data.Repositories;
using ClienteAPI.Identity;
using ClienteAPI.Services.Implementations;
using ClienteAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System; 
var builder = WebApplication.CreateBuilder(args);

// Adiciona e configura a pol�tica de CORS para permitir requisi��es de qualquer origem.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Adiciona suporte a controladores na aplica��o.
builder.Services.AddControllers();

// Adiciona servi�os para explora��o de endpoints da API (usado pelo Swagger).
builder.Services.AddEndpointsApiExplorer();

// Configura��o do Swagger/Swashbuckle para documenta��o da API e suporte a JWT (Bearer Token).
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "ClienteAPI", Version = "v1" });

    // Define o esquema de seguran�a para JWT (Bearer Token).
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando o esquema Bearer.
                      Insira 'Bearer ' [espa�o] e ent�o seu token no campo abaixo.
                      Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Adiciona o requisito de seguran�a global para todos os endpoints protegidos.
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configura��o da string de conex�o para o banco de dados de dom�nio (Clientes e Logradouros).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A string de conex�o 'DefaultConnection' n�o foi encontrada em appsettings.json.");
}
builder.Services.AddDbContext<ClienteDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configura��o da string de conex�o para o banco de dados de Identity (Usu�rios e Roles).
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");
if (string.IsNullOrEmpty(identityConnectionString))
{
    throw new InvalidOperationException("A string de conex�o 'IdentityConnection' n�o foi encontrada em appsettings.json.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(identityConnectionString));

// Configura��o do ASP.NET Core Identity.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // N�o exige confirma��o de e-mail para login.
    // Configura��es de complexidade de senha (relaxadas para este desafio).
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Usa Entity Framework Core para persistir dados do Identity.
.AddDefaultTokenProviders(); // Adiciona provedores de token para funcionalidades como reset de senha.

// Configura��o da Autentica��o JWT (JSON Web Token).
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("A chave secreta JWT n�o est� configurada em appsettings.json.");
}

builder.Services.AddAuthentication(options =>
{
    // Define o esquema padr�o de autentica��o e desafio como JWT Bearer.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Define os par�metros para valida��o do token JWT.
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true, // Valida o emissor do token.
        ValidateAudience = true, // Valida a audi�ncia do token.
        ValidateLifetime = true, // Valida a data de expira��o do token.
        ValidateIssuerSigningKey = true // Valida a assinatura do token.
    };
});

// Registro dos Reposit�rios na Inje��o de Depend�ncia como Scoped.
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<LogradouroRepository>();

// Registro dos Servi�os na Inje��o de Depend�ncia, utilizando suas interfaces.
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ILogradouroService, LogradouroService>();

var app = builder.Build();

// Configura o pipeline de requisi��es HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisi��es HTTP para HTTPS.
app.UseHttpsRedirection();

// Habilita o roteamento na aplica��o. Deve vir antes de CORS, Autentica��o e Autoriza��o.
app.UseRouting();

// Aplica a pol�tica de CORS definida ("AllowAll").
app.UseCors("AllowAll");

// Habilita a autentica��o e autoriza��o. Devem vir depois de UseRouting e UseCors.
app.UseAuthentication();
app.UseAuthorization();

// Mapeia os controladores da API.
app.MapControllers();

// Inicia a aplica��o web.
app.Run();