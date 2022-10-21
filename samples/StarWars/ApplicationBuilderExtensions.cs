using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace StarWars
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHeaderLocalization(
            this IApplicationBuilder appBuilder)
        {
            var enCulture = new CultureInfo("en");
            var deCulture = new CultureInfo("de");
            var frCulture = new CultureInfo("fr");

            var localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo>
                {
                    enCulture,
                    deCulture,
                    frCulture,
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    enCulture,
                    deCulture,
                    frCulture,
                },
                DefaultRequestCulture = new RequestCulture(deCulture),
                FallBackToParentCultures = true,
                FallBackToParentUICultures = true,
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    // OIDC standard preferred language request parameter
                    new QueryStringRequestCultureProvider {QueryStringKey = "ui_locales"},
                    new AcceptLanguageHeaderRequestCultureProvider()
                }
            };

            appBuilder.UseRequestLocalization(localizationOptions);

            return appBuilder;
        }
    }
}
