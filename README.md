# bugetalibro
buĝetalibro ist das Coding Kata https://ccd-school.de/coding-dojo/application-katas/haushaltsbuch/

![.NET Core](https://github.com/stefc/bugetalibro/workflows/.NET%20Core/badge.svg)


## Migrations

Tool installieren:

```
dotnet tool install --global dotnet-ef --version 3.1.7
dotnet tool update --global dotnet-ef --version 3.1.7
```

Migrations Befehle 

```
cd backend/TXS.bugetalibro.Infrastructure
dotnet ef migrations list --startup-project ../../TXS.bugetalibro.ConsoleApp
```


```
dotnet ef migrations add <name> --startup-project ../../TXS.bugetalibro.ConsoleApp/ -o Persistence/Migrations
```