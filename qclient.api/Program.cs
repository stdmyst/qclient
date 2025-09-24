var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var users = new User[]
{
    new User(1, "John", "john.due@qclient.com"),
    new User(2, "Chad", "chad.due@qclient.com")
};

app.MapGet("/api/user", (int id) =>
    {
        var user = users.FirstOrDefault(x => x.Id == id);
        if (user == null)
            return Results.NotFound();
        return Results.Ok(user);
    }) 
    .WithName("GetUser")
    .WithOpenApi();

app.MapGet("/api/users",()=> Results.Ok(users))
    .WithName("GetUsers")
    .WithOpenApi();

app.Run();

record User(int Id, string Name, string Email)
{
}