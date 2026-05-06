using System.Text;
using System.Text.Json.Serialization;
using ApiEcommerce.Constants;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1024 * 1024 = 1MByte
    options.UseCaseSensitivePaths = true;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// AutoMapper
// builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAutoMapper(cfg =>
{
    // Escanea todos los perfiles en el ensamblado de Program
    cfg.AddMaps(typeof(Program).Assembly);
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("SecretKey no está configurada");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // false - desactiva Https
    options.SaveToken = true; // guarda el token en el contesto de la autenticación
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,  // que este firmado con una clave valida
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),  // establecemos la clave secreta
        ValidateIssuer = false,  // false - no se valida el emisor del token
        ValidateAudience = false  // false - no se va a valida el publico del token
    };


});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers(option =>
{
    // Versión a pelo
    //option.CacheProfiles.Add("Default10", new CacheProfile()
    // {
    //     Duration = 10
    // });
    //option.CacheProfiles.Add("Default20", new CacheProfile()
    // {
    //     Duration = 20
    // });

    // Vesión con clase de constantes (CacheProfiles)
    option.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
    option.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
});
// ------------- SwaggerGen -------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                      "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                      "Ejemplo: \"12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Ecommerce",
        Description = "API para gestionar productos y usuarios",
        TermsOfService = new Uri("http://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "DevTalles",
            Url = new Uri("https://devtalles.com")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de uso",
            Url = new Uri("http://example.com/license")
        }
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "API Ecommerce v2",
        Description = "API para gestionar productos y usuarios",
        TermsOfService = new Uri("http://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "DevTalles",
            Url = new Uri("https://devtalles.com")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de uso",
            Url = new Uri("http://example.com/license")
        }
    });

});


// versiones de APIs
var apiVersioningBuilder = builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
    // option.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); // ?api-version
});
apiVersioningBuilder.AddApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV"; // v1, v2, v3 ...
    option.SubstituteApiVersionInUrl = true;  // api/v{version}/produts
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyName.AllowSpecificOrigin,
    builder =>
    {
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // ------------- SwaggerGen -------------
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}


Todo[] sampleTodos =
[
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
];

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos)
        .WithName("GetTodos");

todosApi.MapGet("/{id}", Results<Ok<Todo>, NotFound> (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? TypedResults.Ok(todo)
        : TypedResults.NotFound())
    .WithName("GetTodoById");


app.UseHttpsRedirection();


// middleware CORS
app.UseCors(PolicyName.AllowSpecificOrigin);

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
