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
            // Register NutritionPart
            services.AddContentPart<NutritionPart>()
                    .UseDisplayDriver<NutritionPartDisplayDriver>();

            // Register RecipePart
            services.AddContentPart<RecipePart>()
                    .UseDisplayDriver<RecipePartDisplayDriver>();

            // Register Migration
            services.AddDataMigration<Migrations>();
        }
    }
}