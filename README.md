Tuto para clear README
https://www.aluracursos.com/blog/como-escribir-un-readme-increible-en-tu-github
https://docs.github.com/es/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-readmes



## Dev
# instalar
1. Descargar .NET --> https://dotnet.microsoft.com/es-es/download
2. Docker Desktop --> https://www.docker.com/products/docker-desktop/
3. Instalar  Entity Framework --> dotnet tool install --global dotnet-ef
4. Instalar paquete Design (tambien se puede mediante Nuget)
   --> dotnet add package Microsoft.EntityFrameworkCore.Design
5. Migraciones de Entity Framework
   5.1 dotnet ef migrations add InitialMigration (añadimos migraciones sin actualizar la bd como un commit)
   5.2 dotnet ef database update (Actualizar bbdd con las migraciones pendientes)
6. Instalar Nuget --> Microsoft.AspNetCore.Authentication.JwtBearer
7. Instalar Nuget --> Asp.Versioning.Mvc
8. Instalar Nuget --> Asp.Versioning.Mvc.ApiExplorer
9. 



---------------- Entity Framework Core ------------------
https://learn.microsoft.com/es-es/ef/core/
Introducción a las relaciones https://learn.microsoft.com/es-mx/ef/core/modeling/relationships

https://learn.microsoft.com/es-es/ef/core/cli/dotnet#update-the-tools
Use dotnet tool update --global dotnet-ef para actualizar las herramientas globales a la versión más reciente disponible. Si tiene instaladas las herramientas localmente en el proyecto, use dotnet tool update dotnet-ef. Para instalar una versión específica, anexe --version <VERSION> al comando. Vea la sección Actualización de la documentación de la herramienta dotnet para obtener más detalles.

Actualizar herraminta --> dotnet tool update --global dotnet-ef

Comandos de terminar para Entity Framework Core
Añadir tabla nueva --> dotnet ef migrations add CreateTableProduct
Eliminar la ultima migración --> dotnet ef migrations remove
Eliminar la BBDD --> dotnet ef database drop y luego --> dotnet ef database update
---------------------------------------------------
*Actualizar bbdd con las migraciones pendientes --> dotnet ef database update
---------------------------------------------------

------------------------ Swagger ------------------------
Sí, parece que Swagger no viene instalado por defecto, 
en este caso puedes instalarlo manualmente, 
por lo que tienes que ejecutar los siguientes pasos:

1. Ejecutar dotnet add package Swashbuckle.AspNetCore para instalarlo.

2. Y en el program.cs vas a añadir lo siguiente:

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
if (app.Environment.IsDevelopment()) {    
   app.UseSwagger();
   app.UseSwaggerUI(); 
}
Así debería funcionar.

-------------------------- Git -------------------------
Para dejar de seguir una carpeta en Git que ya se seguia
git rm -r --cached nombre_de_tu_carpeta

## instalar paquetes NuGet
BCrypt.Net-Next -> https://www.nuget.org/packages/BCrypt.Net-Next
https://github.com/BcryptNet/bcrypt.net




---------------- SQL SERVER (no me ha funcionado) --------------
Modificar un ID específico (Update directo)
Si necesitas cambiar el valor de un ID en una fila particular, 
debes desactivar temporalmente la restricción IDENTIT

-- 1. Permitir insertar valores explícitos en la columna identidad
SET IDENTITY_INSERT NombreTabla ON;

-- 2. Realizar el update
UPDATE NombreTabla
SET ID = NuevoID
WHERE ID = IDActual;

-- 3. Desactivar la inserción explícita
SET IDENTITY_INSERT NombreTabla OFF;
------------------------------------------------
Alta usuario con Identity
{
  "username": "devi-admin",
  "name": "Devi",
  "password": "Devi123456*",
  "role": "Admin"
}

{
  "username": "jose-admin",
  "name": "Jose",
  "password": "Jose123456*",
  "role": "Admin"
}