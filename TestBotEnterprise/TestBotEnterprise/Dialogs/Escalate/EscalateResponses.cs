// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using TestBotEnterprise.Dialogs.Escalate.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.TemplateManager;
using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace TestBotEnterprise.Dialogs.Escalate
{
    public class EscalateResponses : TemplateManager
    {
        private LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                { ResponseIds.SendPhoneMessage, (context, data) => BuildEscalateCard(context, data) },
            }
        };

        public EscalateResponses()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }

        public static IMessageActivity BuildEscalateCard(ITurnContext turnContext, dynamic data)
        {
            var attachment = new HeroCard()
            {
                Text = EscalateStrings.PHONE_INFO,
                Buttons = new List<CardAction>()
                {
                    new CardAction(type: ActionTypes.OpenUrl, title: "Call now", value: "tel:08008005383"),
                    // JU debemos investigar la posibilidad de sumar un agente a este mismo chat, no de llevar al cliente a otro canal.
                    // new CardAction(type: ActionTypes.OpenUrl, title: "Open Teams", value: "msteams://")
                },
            }.ToAttachment();

            return MessageFactory.Attachment(attachment, null, EscalateStrings.PHONE_INFO, InputHints.AcceptingInput);
        }

        public class ResponseIds
        {
            public const string SendPhoneMessage = "sendPhoneMessage";
        }
    }
}
