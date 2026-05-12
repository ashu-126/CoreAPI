using CoreAPI.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace CoreAPI
{
    public class Startup
    {
        private readonly Dictionary<string, string> connectionString = new Dictionary<string, string>()
        {
            { "CoreAPIDevDatabase", "Server=(localdb)\\MSSQLLocalDB;Database=JwtRoleApiDb;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False" },

        };

        /// <summary>
		/// Startup constructor
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="environment"></param>
		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
        }

        /// <summary>
        /// Define configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Define hosting environment
        /// </summary>
        public IWebHostEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Add services to the container
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Startup).Assembly));

            services.AddAuthorization();

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CoreAPI",
                    Version = "v1",
                    Description = "Core API Documentation"
                });
            });

            //Database instance registation
            services.AddDbContext<Context>(db =>
            {
                string connString = connectionString[Configuration["ConnectionString"]];
                if(connString != null)
                {
                    db.UseSqlServer(connString);
                }
                else
                {
                    ConsoleColor color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Error");
                    Console.ForegroundColor = color;
                    Console.Write("Could not find database connection string");
                    Console.WriteLine();
                }
            });

            // ── JWT Authentication ─────────────────────────────────
            var jwt = Configuration.GetSection("JwtSettings");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt["SecretKey"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });


            services.AddRepositories();
        }

        /// <summary>
        /// Configure the HTTP request pipeline — THIS WAS MISSING
        /// </summary>
        /// This is the middleware pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable Swagger middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreAPI v1");
                c.RoutePrefix = string.Empty; // opens swagger at root → https://localhost:{port}/
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();  // include if using JWT auth
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
