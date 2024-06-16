using MassTransit;
using Menu.Application.UseCases.Consumers;
using MessageBus.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Infrastructure.Data;
using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OutOfOfficeDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.ValidateIssuer = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateLifetime = false;
        options.TokenValidationParameters.RequireExpirationTime = false;
        options.TokenValidationParameters.RequireSignedTokens = false;
        options.TokenValidationParameters.RequireAudience = false;
        options.TokenValidationParameters.ValidateActor = false;
        options.TokenValidationParameters.ValidateIssuerSigningKey = false;

        options.TokenValidationParameters.SignatureValidator = delegate (string token, TokenValidationParameters parameters)
        {
            var jwtHandler = new JsonWebTokenHandler();
            var jsonToken = jwtHandler.ReadJsonWebToken(token);
            return jsonToken;
        };
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TempData"));

        var jwtBearerSettings = builder.Configuration.GetSection("JwtBearer");
        options.Authority = jwtBearerSettings["Authority"];
        options.Audience = "OutOfOffice.WebApi";
    });
builder.Services.AddAuthorization(options =>
{



    options.AddPolicy("RequireEmployeeRole", policy =>
        policy.RequireAssertion(context =>
                context.User.IsInRole("Employee")  || context.User.IsInRole("Administrator")));

    options.AddPolicy("RequireHRManagerRole", policy =>
        policy.RequireAssertion(context =>
                context.User.IsInRole("HRManager")  || context.User.IsInRole("Administrator")));

    options.AddPolicy("RequirePMManagerRole", policy =>
         policy.RequireAssertion(context =>
               context.User.IsInRole("PMManager") || context.User.IsInRole("Administrator")));

    options.AddPolicy("RequireAdministratorRole", policy =>
        policy.RequireRole("Administrator"));

    options.AddPolicy("RequireHROrPMManagerRole", policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("HRManager") || context.User.IsInRole("PMManager") || context.User.IsInRole("Administrator")));
});
builder.Services.AddCors();

builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblies(typeof(FinishUserRegistrationCommand).Assembly);

});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreation_Consumer>();

    x.UsingRabbitMq((cxt, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<UserPositionSetEvent>(p => p.ExchangeType = ExchangeType.Fanout);
        cfg.Publish<UserChangedEvent>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.ReceiveEndpoint("OutOfOffice_UserConsumer_queue", e =>
        {
            e.ConfigureConsumer<UserCreation_Consumer>(cxt);
        });
    });

});


var app = builder.Build();
app.UseCors(policyBuilder =>
{
    policyBuilder.WithOrigins("https://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
