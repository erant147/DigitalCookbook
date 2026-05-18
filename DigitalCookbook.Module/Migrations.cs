using System.Threading.Tasks;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace DigitalCookbook.Module
{
    public class Migrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public async Task<int> CreateAsync()
        {
            // 1. Define NutritionPart
            await _contentDefinitionManager.AlterPartDefinitionAsync("NutritionPart", part => part
                .WithField("Calories", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Calories")
                    .WithSettings(new { Position = "1" }))
                .WithField("AllergenInfo", field => field
                    .OfType("TextField")
                    .WithDisplayName("Allergen Info")
                    .WithSettings(new { Position = "2" }))
            );

            // 2. Define RecipePart with its fields
            await _contentDefinitionManager.AlterPartDefinitionAsync("RecipePart", part => part
                .WithField("RecipePhotos", field => field
                    .OfType("MediaField")
                    .WithDisplayName("Recipe Photos")
                    .WithSettings(new { Multiple = true, Position = "1" }))
                .WithField("IngredientsList", field => field
                    .OfType("ContentPickerField")
                    .WithDisplayName("Ingredients")
                    .WithSettings(new
                    {
                        DisplayedContentTypes = new[] { "Ingredient" },
                        Multiple = true,
                        Position = "2"
                    }))
            );

            // 3. Define Ingredient Content Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Ingredient", type => type
                .DisplayedAs("Ingredient")
                .Creatable()
                .Listable()
                .Draftable()
                .Versionable()
                .WithPart("TitlePart", part => part.WithPosition("0"))
                .WithPart("NutritionPart", part => part.WithPosition("1"))
            );

            // 4. Define Recipe Content Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Recipe", type => type
                .DisplayedAs("Recipe")
                .Creatable()
                .Listable()
                .Draftable()
                .Versionable()
                .WithPart("TitlePart", part => part.WithPosition("0"))
                .WithPart("RecipePart", part => part.WithPosition("1"))
                .WithPart("HtmlBodyPart", part => part
                    .WithDisplayName("Cooking Steps")
                    .WithPosition("2"))
            );

            return 1;
        }
    }
}