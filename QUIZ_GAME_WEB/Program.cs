// ------------------------------------------------------------------
// FILE: Program.cs (ĐÃ SỬA LỖI GỌI SeedData.Initialize)
// ------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection; // Cần cho ILogger

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
        // ! được dùng để đảm bảo giá trị không null khi sử dụng tính năng nullability
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// === 4. CẤU HÌNH SWAGGER GEN ===
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "QUIZ_GAME_WEB API", Version = "v1" });

    // 1. Định nghĩa Security Scheme (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
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

var app = builder.Build();

// === 5. GỌI SEEDDATA SAU KHI BUILD (ĐÃ SỬA LỖI) ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // 📢 KHẮC PHỤC LỖI: Gọi phương thức Initialize mới thêm vào lớp SeedData
        SeedData.Initialize(services);

    }
    catch (Exception ex)
    {
        // Thêm ILogger để log lỗi (cần using System.Reflection; hoặc Microsoft.Extensions.Logging)
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Một lỗi đã xảy ra khi seed database.");
    }
}

// === 6. CẤU HÌNH HTTP PIPELINE ===
// Ghi chú: app.UseRouting() thường được gọi trước UseCors nếu Cors là middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

// Bật xác thực VÀ phân quyền (ĐÚNG THỨ TỰ)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();