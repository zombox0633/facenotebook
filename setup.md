## create project
dotnet new webapi -n ชือ

## run project
dotnet run

## เพิ่ม Swagger
dotnet add package Swashbuckle.AspNetCore

http://localhost:5268/swagger/index.html

## recheck มั้ง
dotnet restore


## docker postgres
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design

## check IP Address
ipconfig