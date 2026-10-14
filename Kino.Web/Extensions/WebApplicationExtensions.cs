namespace Kino.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApplicationLocalization(this WebApplication app)
    {
        // налаштування (мови, порядок провайдерів) задані в AddApplicationLocalization
        app.UseRequestLocalization();

        return app;
    }

    public static WebApplication UseApplicationErrorHandling(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Home/StatusCodeHandler/{0}");

        return app;
    }
}
