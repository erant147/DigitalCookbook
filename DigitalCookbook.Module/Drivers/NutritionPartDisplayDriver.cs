using System.Threading.Tasks;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using DigitalCookbook.Module.Models;
using OrchardCore.ContentManagement.Display.Models;

namespace DigitalCookbook.Module.Drivers
{
    public class NutritionPartDisplayDriver : ContentPartDisplayDriver<NutritionPart>
    {
        public override IDisplayResult Edit(NutritionPart part, BuildPartEditorContext context)
        {
            return View("NutritionPart_Edit", part).Location("Content:1");
        }

        public override IDisplayResult Display(NutritionPart part, BuildPartDisplayContext context)
        {
            if (context.DisplayType == "SummaryAdmin")
            {
                return null;
            }

            return View("NutritionPart_Display", part).Location("Content:5");
        }

        public override async Task<IDisplayResult> UpdateAsync(NutritionPart part, UpdatePartEditorContext context)
        {
            await context.Updater.TryUpdateModelAsync(part, Prefix, t => t.Calories, t => t.AllergenInfo);
            return Edit(part, context);
        }
    }
}