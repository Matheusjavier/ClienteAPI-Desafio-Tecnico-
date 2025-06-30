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

// Adiciona e configura a política de CORS para permitir requisições de qualquer origem.
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

// Adiciona suporte a controladores na aplicação.
builder.Services.AddControllers();

// Adiciona serviços para exploração de endpoints da API (usado pelo Swagger).
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger/Swashbuckle para documentação da API e suporte a JWT (Bearer Token).
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "ClienteAPI", Version = "v1" });

    // Define o esquema de segurança para JWT (Bearer Token).
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando o esquema Bearer.
                      Insira 'Bearer ' [espaço] e então seu token no campo abaixo.
                      Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Adiciona o requisito de segurança global para todos os endpoints protegidos.
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

// Configuração da string de conexão para o banco de dados de domínio (Clientes e Logradouros).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada em appsettings.json.");
}
builder.Services.AddDbContext<ClienteDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuração da string de conexão para o banco de dados de Identity (Usuários e Roles).
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");
if (string.IsNullOrEmpty(identityConnectionString))
{
    throw new InvalidOperationException("A string de conexão 'IdentityConnection' não foi encontrada em appsettings.json.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(identityConnectionString));

// Configuração do ASP.NET Core Identity.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Não exige confirmação de e-mail para login.
    // Configurações de complexidade de senha (relaxadas para este desafio).
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Usa Entity Framework Core para persistir dados do Identity.
.AddDefaultTokenProviders(); // Adiciona provedores de token para funcionalidades como reset de senha.

// Configuração da Autenticação JWT (JSON Web Token).
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("A chave secreta JWT não está configurada em appsettings.json.");
}

builder.Services.AddAuthentication(options =>
{
    // Define o esquema padrão de autenticação e desafio como JWT Bearer.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Define os parâmetros para validação do token JWT.
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true, // Valida o emissor do token.
        ValidateAudience = true, // Valida a audiência do token.
        ValidateLifetime = true, // Valida a data de expiração do token.
        ValidateIssuerSigningKey = true // Valida a assinatura do token.
    };
});

// Registro dos Repositórios na Injeção de Dependência como Scoped.
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<LogradouroRepository>();

// Registro dos Serviços na Injeção de Dependência, utilizando suas interfaces.
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ILogradouroService, LogradouroService>();

var app = builder.Build();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP para HTTPS.
app.UseHttpsRedirection();

// Habilita o roteamento na aplicação. Deve vir antes de CORS, Autenticação e Autorização.
app.UseRouting();

// Aplica a política de CORS definida ("AllowAll").
app.UseCors("AllowAll");

// Habilita a autenticação e autorização. Devem vir depois de UseRouting e UseCors.
app.UseAuthentication();
app.UseAuthorization();

// Mapeia os controladores da API.
app.MapControllers();

// Inicia a aplicação web.
app.Run();