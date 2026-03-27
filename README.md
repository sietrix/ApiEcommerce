Tuto para clear README
https://www.aluracursos.com/blog/como-escribir-un-readme-increible-en-tu-github
https://docs.github.com/es/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-readmes







---------------- Entity Framework Core ------------------
https://learn.microsoft.com/es-es/ef/core/
Introducción a las relaciones https://learn.microsoft.com/es-mx/ef/core/modeling/relationships

Comandos de terminar para Entity Framework Core
Añadir tabla nueva --> dotnet ef migrations add CreateTableProduct
Eliminar la ultima migración --> dotnet ef migrations remove
---------------------------------------------------
Actualizar bbdd con las migraciones pendientes --> dotnet ef database update
---------------------------------------------------


------------------------ Swagger ------------------------
Sí, parece que Swagger no viene instalado por defecto, en este caso puedes instalarlo manualmente, por lo que tienes que ejecutar los siguientes pasos:

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
---------------------------------------------------


