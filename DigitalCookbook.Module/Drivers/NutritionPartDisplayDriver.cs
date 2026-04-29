using System.Threading.Tasks;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using DigitalCookbook.Module.Models;

namespace DigitalCookbook.Module.Drivers;

public class NutritionPartDisplayDriver : ContentPartDisplayDriver<NutritionPart>
{
    // This tells Orchard to show the "Summary" or "Detail" view (Display mode)
    public override IDisplayResult Display(NutritionPart part)
    {
        // Use Initialize<NutritionPart> instead of the missing ViewModel
        return Initialize<NutritionPart>("NutritionPart", model =>
        {
            model.Calories = part.Calories;
            model.AllergenInfo = part.AllergenInfo;
            model.ContentItem = part.ContentItem;
        })
        .Location("Detail", "Content:10")
        .Location("Summary", "Content:10");
    }

    // This tells Orchard to show the editor boxes (Edit mode)
    public override IDisplayResult Edit(NutritionPart part)
    {
        return Initialize<NutritionPart>("NutritionPart_Edit", model =>
        {
            model.Calories = part.Calories;
            model.AllergenInfo = part.AllergenInfo;
        })
        .Location("Content:10");
    }

    // This saves the data from the text boxes back to the database
    public override async Task<IDisplayResult> UpdateAsync(NutritionPart part, IUpdateModel updater)
    {
        // We use "NutritionPart" here because that is the name of the Part itself.
        // Orchard uses this name to find the values in the web form.
        await updater.TryUpdateModelAsync(part, "NutritionPart", t => t.Calories, t => t.AllergenInfo);

        return Edit(part);
    }
}