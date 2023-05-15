namespace StorageAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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

            builder.Services.AddDbContext<StorageContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<AbstractValidator<AuthRequest>, AuthRequestValidator>();
            builder.Services.AddScoped<AbstractValidator<AddTextRequest>, AddTextRequestValidator>();

            builder.Services.AddSingleton<PasswordHasher<User>>();

            builder.Services.AddScoped<IUserRepository, DbUserRepository>();
            builder.Services.AddScoped<ITextRepository, DbTextRepository>();
            builder.Services.AddScoped<IFileDataRepository, DbFileDataRepository>();

            builder.Services.AddScoped<IFileStorageService, OsFileStorageService>();

            builder.Services.AddTransient<IApi, AuthApi>();
            builder.Services.AddTransient<IApi, TextsApi>();
            builder.Services.AddTransient<IApi, FilesApi>();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            using var scope = app.Services.CreateScope();
            var apis = scope.ServiceProvider.GetServices<IApi>();
            foreach (var api in apis)
            {
                api.Configure(app);
            }

            app.Run();
        }
    }
}