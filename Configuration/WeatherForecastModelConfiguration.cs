using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.OData.ModelBuilder;

namespace odata_groupby
{
    public class WeatherForecastModelConfiguration : IModelConfiguration
    {
        private static EntityTypeConfiguration<WeatherForecast> ConfigureCurrent(ODataModelBuilder builder)
        {
            var entity = builder.EntitySet<WeatherForecast>("WeatherForecasts").EntityType;

            return entity;
        }

        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
        {
            if (routePrefix != "api/v{version:apiVersion}")
            {
                return;
            }

            ConfigureCurrent(builder);
        }
    }
}
