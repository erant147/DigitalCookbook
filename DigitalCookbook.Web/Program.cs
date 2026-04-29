namespace DigitalCookbook.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Orchard CMS services
            builder.Services.AddOrchardCms();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            // This activates the Orchard middleware
            app.UseOrchardCore();

            app.Run();
        }
    }
}
