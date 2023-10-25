using Data.Entities;
using Azure.Identity;
using Data.Repositories.UserRepo;
using Data.Repositories.PostRepo;
using Business.Services.PostServices;
using Business.Services.CommentServices;
using Business.Services.UserServices;
using Business.Services.EmailServices;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Business.Services.SecretServices;
using Data.Repositories.OTPRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Data.Repositories.UserFollowingRepo;
using Business.Services.UserFollowingServices;
using Data.Repositories.PostAttachmentRepo;
using Data.Repositories.PostReactRepo;
using Data.Repositories.PetPostTradeRepo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "PetLovers.API",
        Description = "APIs for PetLovers"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. " +
                            "\n\nEnter your token in the text input below. " +
                              "\n\nExample: '12345abcde'",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Connect Database 

builder.Services.AddDbContext<PetLoversDbContext>(option => option.UseSqlServer(SecretService.GetConnectionString()));

// Subcribe service
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IPostServices, PostServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped<ICommentServices, CommentServices>();
builder.Services.AddScoped<IUserFollowingServices, UserFollowingServices>();


//Subcribe repository
builder.Services.AddTransient<IUserRepo, UserRepo>();
builder.Services.AddTransient<IPostRepo, PostRepo>();
builder.Services.AddTransient<IOTPRepo, OTPRepo>();
builder.Services.AddTransient<IUserFollowingRepo, UserFollowingRepo>();
builder.Services.AddTransient<IPostAttachmentRepo, PostAttachmentRepo>();
builder.Services.AddTransient<IPostReactionRepo, PostReactionRepo>();
builder.Services.AddTransient<IPetPostTradeRepo, PetPostTradeRepo>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretService.GetJWTKey())),
        ValidateIssuer = true,
        ValidIssuer = SecretService.GetJWTIssuser(),
        ValidateAudience = false,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
