using System.Text;
//using backend;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using backend;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddTransient(_ => new Database(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//setting the connection string for development environment
if (app.Environment.IsDevelopment()){
app.Use(async (contex, next)=>
{
    /*May need to change database=xxxxxx as we will use a different one, but works right now  */
    System.Environment.SetEnvironmentVariable("DATABASE_URL", "server=127.0.0.1;user id=libraryuser;password=librarypass;port=3306;database=library;");

    Console.WriteLine(System.Environment.GetEnvironmentVariable("DATABASE_URL"));
    await next();
}
);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
