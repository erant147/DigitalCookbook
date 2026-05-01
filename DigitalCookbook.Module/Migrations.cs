using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System.Threading.Tasks;

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
            // 1. Define NutritionPart with Display Settings for the "View" screen
            await _contentDefinitionManager.AlterPartDefinitionAsync("NutritionPart", part => part
                .WithField("Calories", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Calories")
                    .WithSettings(new { Position = "1" }))
                .WithField("Protein", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Protein")
                    .WithSettings(new { Position = "2" }))
                .WithField("Carbohydrates", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Carbohydrates")
                    .WithSettings(new { Position = "3" }))
                .WithField("Fat", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Fat")
                    .WithSettings(new { Position = "4" }))
            );

            await _contentDefinitionManager.AlterTypeDefinitionAsync("Ingredient", type => type
                .DisplayedAs("Ingredient")
                .Creatable().Listable()
                .WithPart("TitlePart", part => part.WithSettings(new { RenderTitle = true }))
                .WithPart("NutritionPart")
            );

            // 2. Build the Recipe Type
            await BuildRecipeType();

            return 1;
        }

        private async Task BuildRecipeType()
        {
            // 1. Define the Fields on the Part first
            await _contentDefinitionManager.AlterPartDefinitionAsync("Recipe", part => part
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

            // 2. Then attach that Part to the Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Recipe", type => type
                .DisplayedAs("Recipe")
                .Creatable().Listable().Draftable().Versionable()
                .WithPart("TitlePart", part => part.WithPosition("1"))
                .WithPart("Recipe", part => part.WithPosition("2"))
                .WithPart("HtmlBodyPart", part => part
                    .WithDisplayName("Cooking Steps")
                    .WithPosition("3"))
            );
        }

        // Incrementing versions to force Orchard to run the new logic
        public async Task<int> UpdateFrom1Async() { await CreateAsync(); return 2; }
        public async Task<int> UpdateFrom2Async() { await CreateAsync(); return 3; }
        public async Task<int> UpdateFrom3Async() { await CreateAsync(); return 4; }
        public async Task<int> UpdateFrom4Async() { await CreateAsync(); return 5; }
        public async Task<int> UpdateFrom5Async() { await CreateAsync(); return 6; }
        public async Task<int> UpdateFrom6Async() { await CreateAsync(); return 7; }
        public async Task<int> UpdateFrom7Async() { await CreateAsync(); return 8; }
        public async Task<int> UpdateFrom8Async() { await CreateAsync(); return 9; }
        public async Task<int> UpdateFrom9Async()
        {
            await BuildRecipeType();
            await CreateAsync(); // This ensures the NutritionPart settings also update
            return 10;
        }
    }
}