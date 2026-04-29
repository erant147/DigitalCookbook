using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using DigitalCookbook.Module.Models;
using DigitalCookbook.Module.Drivers;
using OrchardCore.ContentManagement.Display.ContentDisplay;

namespace DigitalCookbook.Module
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddContentPart<NutritionPart>()
                    .UseDisplayDriver<NutritionPartDisplayDriver>();

            services.AddDataMigration<Migrations>();
        }
    }
}