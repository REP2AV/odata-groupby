using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;

namespace odata_groupby
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                string? cs = "Server=(localdb)\\mssqllocaldb;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true";

                if (string.IsNullOrEmpty(cs))
                {
                    throw new Exception();
                }

                options.UseSqlServer(cs);

                if (!builder.Environment.IsProduction())
                {
                    options.EnableSensitiveDataLogging();
                    options.UseLoggerFactory(LoggerFactory.Create(builder =>
                    {
                        builder.AddConsole();
                    }));
                }
            });

            builder.Services
                .AddControllers()
                .AddOData(options =>
                {
                    options.EnableQueryFeatures();
                    options.EnableNoDollarQueryOptions = false;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services
                .AddApiVersioning(options =>
                {
                    // reporting api versions will return the headers
                    // "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                })
                .AddOData(options =>
                {
                    options.AddRouteComponents("api/v{version:apiVersion}");

                    options.ModelBuilder.ModelBuilderFactory = () => new ODataConventionModelBuilder();
                })
                .AddODataApiExplorer(options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<ApplicationDbContext>();

                context.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.

            app.UseRouting();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}