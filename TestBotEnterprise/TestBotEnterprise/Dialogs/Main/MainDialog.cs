﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestBotEnterprise.Dialogs.Escalate;
using TestBotEnterprise.Dialogs.Onboarding;
using TestBotEnterprise.Dialogs.Shared;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TestBotEnterprise.Dialogs.Main
{
    public class MainDialog : RouterDialog
    {
        private BotServices _services;
        private UserState _userState;
        private ConversationState _conversationState;
        private MainResponses _responder = new MainResponses();

        public MainDialog(BotServices services, ConversationState conversationState, UserState userState, IBotTelemetryClient telemetryClient)
            : base(nameof(MainDialog))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _conversationState = conversationState;
            _userState = userState;
            TelemetryClient = telemetryClient;

            AddDialog(new OnboardingDialog(_services, _userState.CreateProperty<OnboardingState>(nameof(OnboardingState)), telemetryClient));
            AddDialog(new EscalateDialog(_services));
        }

        protected override async Task OnStartAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var view = new MainResponses();
            // await view.ReplyWith(dc.Context, MainResponses.ResponseIds.Intro);
            await view.ReplyWith(dc.Context, MainResponses.ResponseIds.Greeting);
        }

        protected override async Task RouteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Check dispatch result
            var dispatchResult = await _services.DispatchRecognizer.RecognizeAsync<Dispatch>(dc, CancellationToken.None);
            var intent = dispatchResult.TopIntent().intent;

            System.Diagnostics.Debug.WriteLine("Locale: " + dc.Context.Activity.Locale);

            if (intent == Dispatch.Intent.l_general)
            {
                // If dispatch result is general luis model
                _services.LuisServices.TryGetValue("general", out var luisService);

                if (luisService == null)
                {
                    throw new Exception("The specified LUIS Model could not be found in your Bot Services configuration.");
                }
                else
                {
                    var result = await luisService.RecognizeAsync<General>(dc, CancellationToken.None);

                    var generalIntent = result?.TopIntent().intent;

                    // switch on general intents
                    switch (generalIntent)
                    {
                        case General.Intent.Cancel:
                            {
                                // send cancelled response
                                await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Cancelled);

                                // Cancel any active dialogs on the stack
                                await dc.CancelAllDialogsAsync();
                                break;
                            }

                        case General.Intent.Escalate:
                            {
                                // start escalate dialog
                                await dc.BeginDialogAsync(nameof(EscalateDialog));
                                break;
                            }

                        case General.Intent.Help:
                            {
                                // send help response
                                await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Help);
                                break;
                            }

                        case General.Intent.None:
                        default:
                            {
                                // No intent was identified, send confused message
                                await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Confused);
                                break;
                            }
                    }
                }
            }
            else if (intent == Dispatch.Intent.q_faq)
            {
                _services.QnAServices.TryGetValue("faq", out var qnaService);

                if (qnaService == null)
                {
                    throw new Exception("The specified QnA Maker Service could not be found in your Bot Services configuration.");
                }
                else
                {
                    var answers = await qnaService.GetAnswersAsync(dc.Context);

                    if (answers != null && answers.Count() > 0)
                    {
                        // JU traducir answers[0].Answer
                        var accessToken = await HelperServices.Translator.GetAuthenticationToken("d7074f3cb1e2449580ff25610d5526b0");

                        await dc.Context.SendActivityAsync(await HelperServices.Translator.TranslateText(answers[0].Answer, dc.Context.Activity.Locale, accessToken));
                        // await dc.Context.SendActivityAsync(answers[0].Answer);
                    }
                }
            }
            else if (intent == Dispatch.Intent.q_chitchat)
            {
                _services.QnAServices.TryGetValue("chitchat", out var qnaService);

                if (qnaService == null)
                {
                    throw new Exception("The specified QnA Maker Service could not be found in your Bot Services configuration.");
                }
                else
                {
                    var answers = await qnaService.GetAnswersAsync(dc.Context);

                    if (answers != null && answers.Count() > 0)
                    {
                        // JU traducir answers[0].Answer
                        var accessToken = await HelperServices.Translator.GetAuthenticationToken("d7074f3cb1e2449580ff25610d5526b0");

                        await dc.Context.SendActivityAsync(await HelperServices.Translator.TranslateText(answers[0].Answer, dc.Context.Activity.Locale, accessToken));
                        // await dc.Context.SendActivityAsync(answers[0].Answer);
                    }
                }
            }
            else
            {
                // If dispatch intent does not map to configured models, send "confused" response.
                await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Confused);
            }
        }

        protected override async Task OnEventAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Check if there was an action submitted from intro card
            if (dc.Context.Activity.Value != null)
            {
                dynamic value = dc.Context.Activity.Value;
                if (value.action == "startOnboarding")
                {
                    await dc.BeginDialogAsync(nameof(OnboardingDialog));
                    return;
                }
            }
        }

        protected override async Task CompleteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // The active dialog's stack ended with a complete status
            // JU: al terminar una acción se pregunta ¿En qué más puedo ayudarte?, esto solo sería necesario para acciones largas.
            // await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Completed);
        }
    }
}
