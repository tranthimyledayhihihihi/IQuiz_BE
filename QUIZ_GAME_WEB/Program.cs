using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // <-- Thêm dòng này

var builder = WebApplication.CreateBuilder(args);

// === 1. ĐỊNH NGHĨA CHÍNH SÁCH CORS ===
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// === 2. THÊM DBCONTEXT ===
builder.Services.AddDbContext<QuizGameContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === 3. CẤU HÌNH DỊCH VỤ XÁC THỰC (AUTHENTICATION) JWT ===
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// === 4. THAY THẾ 'AddSwaggerGen()' BẰNG KHỐI NÀY ===
builder.Services.AddSwaggerGen(options =>
{
    // Thêm mô tả cho Swagger
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "QUIZ_GAME_WEB API", Version = "v1" });

    // 1. Định nghĩa Security Scheme (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, // Dùng Http
        Scheme = "Bearer", // Ghi rõ là "Bearer"
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập 'Bearer' [dấu cách] và dán token của bạn vào đây.\n\nVí dụ: \"Bearer eyJhbGciOi...\""
    });

    // 2. Thêm yêu cầu bảo mật (thêm ổ khóa vào các API)
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
            new string[] {}
        }
    });
});
// ============================================

var app = builder.Build();

// === 5. GỌI SEEDDATA SAU KHI BUILD ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Một lỗi đã xảy ra khi seed database.");
    }
}

// === 6. CẤU HÌNH HTTP PIPELINE ===
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

// Bật xác thực VÀ phân quyền (ĐÚNG THỨ TỰ)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();