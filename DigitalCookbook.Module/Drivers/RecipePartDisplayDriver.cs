using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using DigitalCookbook.Module.Models;
using OrchardCore.ContentManagement.Display.Models;

namespace DigitalCookbook.Module.Drivers
{
    public class RecipePartDisplayDriver : ContentPartDisplayDriver<RecipePart>
    {
        public override IDisplayResult Display(RecipePart part, BuildPartDisplayContext context)
        {
            return View("RecipePart", part).Location("Content", "Content:10");
        }
    }
}