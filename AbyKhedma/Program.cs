using AbyKhedma.Entities;
using AbyKhedma.Extensions;
using AbyKhedma.Helpers;
using AbyKhedma.Interfaces;
using AbyKhedma.Middlewares;
using AbyKhedma.Pagination;
using AbyKhedma.Persistance;
using AbyKhedma.Services;
using AbyKhedma.SignalRHubs;
using Core.Common;
using Infrastructure.Interceptors;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Interfaces;
using Serilog;
using Services.Services;
using System.Text;
using CorePush.Apple;
using Microsoft.Extensions.Configuration;
using CorePush.Apple;
using CorePush.Google;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using CloudinaryDotNet;
using System.IO.Abstractions;


var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
  .Enrich.FromLogContext()
  .CreateLogger();

var defaultApp = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "abi-khdmh-bff06-firebase-adminsdk-881.json")),
});
builder.Logging.AddSerilog(logger);
// Add services to the container.
//builder.Services.AddEntityFrameworkNpgsql()
//                .AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) =>
//                optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase"))
//                 .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntitiesInterceptor>()),ServiceLifetime.Transient);
builder.Services.AddEntityFrameworkSqlServer()
                .AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) =>
                optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("WebApiDatabaseSqlServer"))
                 .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntitiesInterceptor>()), ServiceLifetime.Transient);


// Add services to the container.
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUriService>(o =>
{
    var accessor = o.GetRequiredService<IHttpContextAccessor>();
    var request = accessor.HttpContext.Request;
    var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
    return new UriService(uri);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://harmonious-banoffee-49c203.netlify.app",
                              "https://dash.abikhdmh.com", "http://abikhdmh.com", "http://localhost:52830")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();/*allow origins (set of links) for signalR*/
                      });
});

builder.Services.AddControllers();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddHttpClient<FcmSender>();
builder.Services.AddHttpClient<ApnSender>();

// Configure strongly typed settings objects
//var appSettingsSection = builder.Configuration.GetSection("FcmNotification");
//builder.Services.Configure<FcmNotificationSetting>(appSettingsSection);






//builder.Services.AddSingleton<ExceptionMiddlewareExtensions>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<SMSMsegat>(builder.Configuration.GetSection("SMSMsegat"));
builder.Services.Configure<NexmoSettings>(builder.Configuration.GetSection("nexmoSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IRequirementRepository, RequirementRepository>();
builder.Services.AddScoped<IStatementRepository, StatementRepository>();
builder.Services.AddScoped<IReelRepository, ReelRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestFlowRepository, RequestFlowRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
builder.Services.AddScoped<ISMSService, SMSService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();


builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<ServiceService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<RequirementService>();
builder.Services.AddScoped<StatementService>();
builder.Services.AddScoped<ReelService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<RequestFlowService>();
builder.Services.AddScoped<ChatMessageService>();
builder.Services.AddScoped<SubAnswerRequirementService>();

builder.Services.AddSingleton<AuditableEntitiesInterceptor>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:JWTTokenSecret").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });
builder.Services.AddSingleton(logger);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseHsts();
app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
//app.ConfigureExceptionHandler(logger);
//app.ConfigureCustomExceptionMiddleware();
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationUserHub>("/NotificationUserHub");
app.Run();
