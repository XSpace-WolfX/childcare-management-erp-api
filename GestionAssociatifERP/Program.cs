using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AutoMapper;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Mappings;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using GestionAssociatifERP.Swagger;
using Microsoft.EntityFrameworkCore;

namespace GestionAssociatifERP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .AllowAnyOrigin() // En prod, tu mettras .WithOrigins("https://ton-domaine.fr")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Ajoute le versioning avec l'explorateur Swagger
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

            if (!builder.Environment.IsEnvironment("Testing"))
                builder.Services.AddDbContext<GestionAssociatifDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSqlConnection")));

            // Injection Repositories
            builder.Services.AddScoped<IResponsableRepository, ResponsableRepository>();
            builder.Services.AddScoped<IEnfantRepository, EnfantRepository>();
            builder.Services.AddScoped<IInformationFinanciereRepository, InformationFinanciereRepository>();
            builder.Services.AddScoped<ISituationPersonnelleRepository, SituationPersonnelleRepository>();
            builder.Services.AddScoped<IDonneeSupplementaireRepository, DonneeSupplementaireRepository>();
            builder.Services.AddScoped<IPersonneAutoriseeRepository, PersonneAutoriseeRepository>();
            builder.Services.AddScoped<IResponsableEnfantRepository, ResponsableEnfantRepository>();
            builder.Services.AddScoped<IPersonneAutoriseeEnfantRepository, PersonneAutoriseeEnfantRepository>();

            // Injection Services
            builder.Services.AddScoped<IResponsableService, ResponsableService>();
            builder.Services.AddScoped<IEnfantService, EnfantService>();
            builder.Services.AddScoped<IInformationFinanciereService, InformationFinanciereService>();
            builder.Services.AddScoped<ISituationPersonnelleService, SituationPersonnelleService>();
            builder.Services.AddScoped<IDonneeSupplementaireService, DonneeSupplementaireService>();
            builder.Services.AddScoped<IPersonneAutoriseeService, PersonneAutoriseeService>();
            builder.Services.AddScoped<ILinkResponsableEnfantService, LinkResponsableEnfantService>();
            builder.Services.AddScoped<ILinkPersonneAutoriseeEnfantService, LinkPersonneAutoriseeEnfantService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                                $"API {description.GroupName.ToUpper()}");
                    }
                });
            }

            app.UseExceptionHandler();

            app.UseStatusCodePages();

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/", () => "Gestion Associatif ERP API is running 🚀");

            app.Run();
        }
    }
}