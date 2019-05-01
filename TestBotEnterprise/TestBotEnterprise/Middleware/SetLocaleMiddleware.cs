// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace TestBotEnterprise.Middleware
{
    public class SetLocaleMiddleware : IMiddleware
    {
        private readonly string defaultLocale;

        public SetLocaleMiddleware(string defaultDefaultLocale)
        {
            defaultLocale = defaultDefaultLocale;
        }

        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            // JU aqui podria llamar al traductor ??
            var accessToken = await HelperServices.Translator.GetAuthenticationToken("d7074f3cb1e2449580ff25610d5526b0");

            context.Activity.Locale = await HelperServices.Translator.DetectText(context.Activity.Text, accessToken);

            context.Activity.Text = await HelperServices.Translator.TranslateText(context.Activity.Text, "es", accessToken);
            //

            var cultureInfo = context.Activity.Locale != null ? new CultureInfo(context.Activity.Locale) : new CultureInfo(defaultLocale);

            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = cultureInfo;

            await next(cancellationToken).ConfigureAwait(false);
        }

    }
}
