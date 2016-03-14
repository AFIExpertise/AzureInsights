using System.Web.Http;
using WebActivatorEx;
using ProductsApp;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace ProductsApp
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "ProductsApp");
                        c.IncludeXmlComments(System.String.Format(@"{0}\bin\AzureInsights.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
        }
    }
}
