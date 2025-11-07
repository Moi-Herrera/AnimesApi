using Microsoft.EntityFrameworkCore;
using AnimesApi.Data;
using AnimesApi.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//ef core 
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//servicio para generar el codigo JWT
builder.Services.AddScoped <JwtService>();


//configuracion 
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, //validar quien creo el token
            ValidateAudience = true,// validar para quien es el token
            ValidateLifetime = true,// tiempo de expirar
            ValidateIssuerSigningKey = true,//validar firma

            //valores obtenidos de appsettings.json
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            //clave para verificar el token
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

//swagger documento visual
if (app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();// autenticacion
app.UseAuthorization();// autorizacion


app.UseHttpsRedirection();
app.MapControllers();
app.Run();
