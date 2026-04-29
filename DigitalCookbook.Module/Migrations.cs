using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Title.Models;

namespace DigitalCookbook.Module
{
    public class Migrations : DataMigration
    {
        private readonly IServiceProvider _serviceProvider;

        public Migrations(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<int> CreateAsync()
        {
            var contentDefinitionManager = _serviceProvider.GetRequiredService<IContentDefinitionManager>();

            // 1. Define the NutritionPart and its fields FIRST
            await contentDefinitionManager.AlterPartDefinitionAsync("NutritionPart", part => part
                .WithField("Calories", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Calories"))
                .WithField("Protein", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Protein"))
                .WithField("Carbohydrates", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Carbohydrates"))
                .WithField("Fat", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Fat"))
            );

            // 2. Then define the Ingredient type using that part
            await contentDefinitionManager.AlterTypeDefinitionAsync("Ingredient", type => type
                .DisplayedAs("Ingredient")
                .Creatable()
                .Listable()
                .WithPart("TitlePart", part => part
                    .WithSettings(new TitlePartSettings { RenderTitle = true }))
                .WithPart("NutritionPart")
            );

            return 1;
        }
    }
}