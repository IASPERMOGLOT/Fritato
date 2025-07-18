using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.MapGet("/", () => Results.Text(
    """
    <!DOCTYPE html>
    <html lang="ru">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Форма отправки имени</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                max-width: 500px;
                margin: 0 auto;
                padding: 20px;
            }
            form {
                display: flex;
                flex-direction: column;
                gap: 10px;
            }
            input, button {
                padding: 8px;
                font-size: 16px;
            }
        </style>
    </head>
    <body>
        <h1>Регистрация</h1>
        <form action="/post" method="post">
            <label for="username">Введите имя:</label>
            <input type="text" id="username" name="username" required>
            <label for="username">Введите электронную почту:</label>
            <input type="text" id="username" name="email" required>
            <label for="username">Введите пароль:</label>
            <input type="text" id="password" name="password" required>
            <button type="submit">Отправить</button>
        </form>
    </body>
    </html>
    """, 
    "text/html"));




app.MapGet("/profile", (HttpContext context) => 
{
    context.Request.Query.TryGetValue("id", out var idValue);
    int.TryParse(idValue, out int userId);

    var db = new DataBase();

    var user = db.people.Find(userId);

    return Results.Text(
        $$"""
    <!DOCTYPE html>
    <html lang="ru">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Мой профиль</title>
        <style>
            body {
                font-family: 'Arial', sans-serif;
                line-height: 1.6;
                margin: 0;
                padding: 0;
                background-color: #f5f5f5;
                color: #333;
            }
            
            .container {
                max-width: 800px;
                margin: 30px auto;
                padding: 20px;
                background: white;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            }
            
            h1 {
                color: #2c3e50;
                border-bottom: 2px solid #eee;
                padding-bottom: 10px;
            }
            
            .profile-info {
                margin: 20px 0;
            }
            
            .info-item {
                margin-bottom: 15px;
            }
            
            .info-label {
                font-weight: bold;
                display: inline-block;
                width: 120px;
            }
            
            .actions {
                margin-top: 30px;
            }
            
            .btn {
                display: inline-block;
                padding: 8px 16px;
                background: #3498db;
                color: white;
                text-decoration: none;
                border-radius: 4px;
                transition: background 0.3s;
            }
            
            .btn:hover {
                background: #2980b9;
            }
        </style>
    </head>
    <body>
        <div class="container">
            <h1>Мой профиль</h1>
            
            <div class="profile-info">
                <div class="info-item">
                    <span class="info-label">Имя:</span>
                    <span id="username">{{user.Name}}</span>
                </div>
                
                <div class="info-item">
                    <span class="info-label">Эл. почта:</span>
                    <span id="email">{{user.Email}}</span>
                </div>

            </div>
        </div>
    </body>
    </html>
    """,
        "text/html");
});


app.MapPost("/post", async (HttpContext context) =>
{
    var username = context.Request.Form["username"];
    var password = context.Request.Form["password"];
    var email = context.Request.Form["email"];

    Person person = new Person() { Name = username, Password = password, Email = email };

    DataBase db = new();
    db.people.Add(person);
    db.SaveChanges();
    Console.WriteLine(1);
    context.Response.Redirect($"/profile?Id={person.Id}");
});

app.Run();




public class DataBase : DbContext
{
    public DataBase()
    {
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public DbSet<Person> people { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=users;Username=postgres;Password=1234");
    }
}




public class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
}