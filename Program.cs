using System.Text;
using System.Text.Json.Serialization;
using ApiEcommerce.Constants;
using ApiEcommerce.Models;
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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(dbConnectionString)
  .UseSeeding((context, _) =>
  {
      var appContext = (ApplicationDbContext)context;
      // Seeding de Roles
      if (!appContext.Roles.Any())
      {
          appContext.Roles.AddRange(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
          );
      }
      // Seeding de Categorías
      if (!appContext.Categories.Any())
      {
          appContext.Categories.AddRange(
            new Category { Name = "Ropa y accesorios", CreationDate = DateTime.Now },
            new Category { Name = "Electrónicos", CreationDate = DateTime.Now },
            new Category { Name = "Deportes", CreationDate = DateTime.Now },
            new Category { Name = "Hogar", CreationDate = DateTime.Now },
            new Category { Name = "Libros", CreationDate = DateTime.Now }
          );
      }
      // Seeding de Usuario Administrador
      if (!appContext.ApplicationUsers.Any())
      {
          var hasher = new PasswordHasher<ApplicationUser>();
          var adminUser = new ApplicationUser
          {
              Id = "admin-001",
              UserName = "admin@admin.com",
              NormalizedUserName = "ADMIN@ADMIN.COM",
              Email = "admin@admin.com",
              NormalizedEmail = "ADMIN@ADMIN.COM",
              EmailConfirmed = true,
              Name = "Administrador"
          };
          adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

          var regularUser = new ApplicationUser
          {
              Id = "user-001",
              UserName = "user@user.com",
              NormalizedUserName = "USER@USER.COM",
              Email = "user@user.com",
              NormalizedEmail = "USER@USER.COM",
              EmailConfirmed = true,
              Name = "Usuario Regular"
          };
          regularUser.PasswordHash = hasher.HashPassword(regularUser, "User123!");

          appContext.ApplicationUsers.AddRange(adminUser, regularUser);
      }
      // Seeding de UserRoles
      if (!appContext.UserRoles.Any())
      {
          appContext.UserRoles.AddRange(
            new IdentityUserRole<string> { UserId = "admin-001", RoleId = "1" }, // Admin
            new IdentityUserRole<string> { UserId = "user-001", RoleId = "2" }   // User
          );
      }

      // Seeding de Productos
      if (!appContext.Products.Any())
      {
          appContext.Products.AddRange(
            new Product
            {
                Name = "Camiseta Básica",
                Description = "Camiseta de algodón 100%",
                Price = 25.99m,
                SKU = "PROD-001-CAM-M",
                Stock = 50,
                CategoryId = 1,
                Category = appContext.Categories.Find(1)!,
                ImgUrl = "https://via.placeholder.com/300x300/FF0000/FFFFFF?text=Camiseta",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Smartphone Galaxy",
                Description = "Teléfono inteligente con 128GB",
                Price = 599.99m,
                SKU = "PROD-002-PHO-BLK",
                Stock = 25,
                CategoryId = 2,
                Category = appContext.Categories.Find(2)!,
                ImgUrl = "https://via.placeholder.com/300x300/0000FF/FFFFFF?text=Smartphone",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Pelota de Fútbol",
                Description = "Pelota oficial FIFA",
                Price = 45.00m,
                SKU = "PROD-003-BAL-WHT",
                Stock = 30,
                CategoryId = 3,
                Category = appContext.Categories.Find(3)!,
                ImgUrl = "https://via.placeholder.com/300x300/00FF00/FFFFFF?text=Pelota",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Lámpara de Mesa",
                Description = "Lámpara LED regulable",
                Price = 89.99m,
                SKU = "PROD-004-LAM-WHT",
                Stock = 15,
                CategoryId = 4,
                Category = appContext.Categories.Find(4)!,
                ImgUrl = "https://via.placeholder.com/300x300/FFFF00/000000?text=Lampara",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "El Quijote",
                Description = "Novela clásica de Cervantes",
                Price = 19.99m,
                SKU = "PROD-005-LIB-ESP",
                Stock = 100,
                CategoryId = 5,
                Category = appContext.Categories.Find(5)!,
                ImgUrl = "https://via.placeholder.com/300x300/800080/FFFFFF?text=Libro",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Jeans Clásicos",
                Description = "Pantalones vaqueros azules",
                Price = 79.99m,
                SKU = "PROD-006-PAN-BLU",
                Stock = 40,
                CategoryId = 1,
                Category = appContext.Categories.Find(1)!,
                ImgUrl = "https://via.placeholder.com/300x300/4169E1/FFFFFF?text=Jeans",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Tablet Pro",
                Description = "Tablet 10.5 pulgadas con stylus incluido",
                Price = 459.99m,
                SKU = "PROD-007-TAB-SIL",
                Stock = 20,
                CategoryId = 2,
                Category = appContext.Categories.Find(2)!,
                ImgUrl = "https://via.placeholder.com/300x300/C0C0C0/000000?text=Tablet",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Zapatillas Running",
                Description = "Zapatillas deportivas para correr",
                Price = 129.99m,
                SKU = "PROD-008-ZAP-BLK",
                Stock = 35,
                CategoryId = 3,
                Category = appContext.Categories.Find(3)!,
                ImgUrl = "https://via.placeholder.com/300x300/000000/FFFFFF?text=Zapatillas",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Cafetera Express",
                Description = "Cafetera automática con molinillo integrado",
                Price = 299.99m,
                SKU = "PROD-009-CAF-BLK",
                Stock = 12,
                CategoryId = 4,
                Category = appContext.Categories.Find(4)!,
                ImgUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=Cafetera",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Programación en C#",
                Description = "Guía completa de programación en C# y .NET",
                Price = 49.99m,
                SKU = "PROD-010-LIB-ESP",
                Stock = 80,
                CategoryId = 5,
                Category = appContext.Categories.Find(5)!,
                ImgUrl = "https://via.placeholder.com/300x300/008B8B/FFFFFF?text=C%23+Book",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Chaqueta Deportiva",
                Description = "Chaqueta impermeable para actividades al aire libre",
                Price = 149.99m,
                SKU = "PROD-011-CHA-NAV",
                Stock = 28,
                CategoryId = 1,
                Category = appContext.Categories.Find(1)!,
                ImgUrl = "https://via.placeholder.com/300x300/000080/FFFFFF?text=Chaqueta",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Auriculares Bluetooth",
                Description = "Auriculares inalámbricos con cancelación de ruido",
                Price = 189.99m,
                SKU = "PROD-012-AUR-BLK",
                Stock = 45,
                CategoryId = 2,
                Category = appContext.Categories.Find(2)!,
                ImgUrl = "https://via.placeholder.com/300x300/1C1C1C/FFFFFF?text=Auriculares",
                CreationDate = DateTime.Now
            }
          );
      }
      appContext.SaveChanges();
  })
);

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

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
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
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
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
    app.UseSwaggerUI(options =>
    {
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

app.UseStaticFiles();

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
