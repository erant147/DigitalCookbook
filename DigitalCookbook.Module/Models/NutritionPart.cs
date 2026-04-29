using OrchardCore.ContentManagement;

namespace DigitalCookbook.Module.Models
{
    public class NutritionPart : ContentPart
    {
        public int Calories { get; set; }
        public string? AllergenInfo { get; set; }
    }
}