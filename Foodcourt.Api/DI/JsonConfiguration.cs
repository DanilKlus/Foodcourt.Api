using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Foodcourt.Api.DI
{
    public static class JsonConfiguration
    {
        public static IMvcBuilder ConfigureJson(this IMvcBuilder app)
        {
            return app
                .AddNewtonsoftJson(options =>
                {

                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };
                });
        }
    }
}