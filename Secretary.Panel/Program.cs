var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => 
    options.AddPolicy(name: "CORS",
            policyBuilder =>
            {
                policyBuilder.AllowCredentials();
                policyBuilder.AllowAnyHeader();
                policyBuilder.AllowAnyMethod();
                policyBuilder.WithOrigins(
                    "http://localhost:4200",
                    "http://localhost:4301", 
                    "http://localhost:30080", 
                    "https://secretary.hrodvitnir.net"
                );
            })
);

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CORS");

app.UseAuthorization();

app.MapControllers();

app.Run();