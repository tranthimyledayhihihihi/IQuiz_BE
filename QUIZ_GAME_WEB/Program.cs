using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Implementations;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===============================================
// 1. CONTROLLERS + JSON
// ===============================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ===============================================
// 2. CORS (DEV: mở hoàn toàn cho Android)
// ===============================================
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            policy
                .WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:4200"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    });
});

// ===============================================
// 3. DATABASE
// ===============================================
builder.Services.AddDbContext<QuizGameContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===============================================
// 4. JWT (HỖ TRỢ WEBSOCKET QUA QUERY access_token)
// ===============================================
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // ✅ WebSocket: /ws/game?access_token=...
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ws"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

// ===============================================
// 5. SWAGGER
// ===============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QUIZ_GAME_WEB API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===============================================
// 6. DI
// ===============================================
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<ISocialRepository, SocialRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITranDauRepository, TranDauRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IOnlineMatchService, OnlineMatchService>();
builder.Services.AddScoped<IQuizAttemptService, QuizAttemptService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IRewardService, RewardService>();

// ✅ WebSocket game server
builder.Services.AddSingleton<ISocketGameServer, SocketGameServer>();

var app = builder.Build();

// ===============================================
// 7. PIPELINE
// ===============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

// ✅ Bật WebSockets
app.UseWebSockets();

app.MapControllers();

// ===============================================
// 8. MAP WEBSOCKET: /ws/game
// ===============================================
app.Map("/ws/game", async (HttpContext context) =>
{
    // chỉ cho websocket handshake
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        return;
    }

    // bắt buộc phải có JWT hợp lệ (đã set ở Authentication)
    if (context.User.Identity?.IsAuthenticated != true)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
        return;
    }

    var server = context.RequestServices.GetRequiredService<ISocketGameServer>();
    await server.Handle(context);
});

app.Run();