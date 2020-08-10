# bugetalibro
buĝetalibro ist das Coding Kata https://ccd-school.de/coding-dojo/application-katas/haushaltsbuch/

![.NET Core](https://github.com/stefc/bugetalibro/workflows/.NET%20Core/badge.svg)


## Migrations
dotnet tool install / update --global dotnet-ef

bugetalibro\backend\TXS.bugetalibro.Infrastructure> dotnet ef migrations add !NameDerMigration! --startup-project ..\..\TXS.bugetalibro.ConsoleApp\ -o Persistence/Migrations