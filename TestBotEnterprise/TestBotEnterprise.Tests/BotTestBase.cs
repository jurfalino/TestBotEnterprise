using System.Collections.Generic;
using System.Threading;
using Autofac;
using TestBotEnterprise;
using TestBotEnterprise.Dialogs.Onboarding;
using TestBotEnterprise.Middleware.Telemetry;
using TestBotEnterprise.Tests.LuisTestUtils;
using TestBotEnterprise.Tests.LUTestUtils;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBotEnterprise.Tests
{
    public class BotTestBase
    {
        public IContainer Container { get; set; }

        public BotServices BotServices { get; set; }

        public ConversationState ConversationState { get; set; }

        public UserState UserState { get; set; }

        public IBotTelemetryClient TelemetryClient { get; set; }

        [TestInitialize]
        public virtual void Initialize()
        {
            var builder = new ContainerBuilder();

            ConversationState = new ConversationState(new MemoryStorage());
            UserState = new UserState(new MemoryStorage());
            TelemetryClient = new NullBotTelemetryClient();
            BotServices = new BotServices()
            {
                DispatchRecognizer = DispatchTestUtil.CreateRecognizer(),
                LuisServices = new Dictionary<string, ITelemetryLuisRecognizer>
                {
                    { "general", GeneralTestUtil.CreateRecognizer() }
                },
                QnAServices = new Dictionary<string, ITelemetryQnAMaker>
                {
                    { "faq", FaqTestUtil.CreateRecognizer() },
                    { "chitchat", ChitchatTestUtil.CreateRecognizer() }
                }
            };

            builder.RegisterInstance(new BotStateSet(UserState, ConversationState));
            Container = builder.Build();
        }

        public TestFlow GetTestFlow()
        {
            var adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(UserState, ConversationState));

            var testFlow = new TestFlow(adapter, async (context, token) =>
            {
                var bot = BuildBot();
                await bot.OnTurnAsync(context, CancellationToken.None);
            });

            return testFlow;
        }

        public IBot BuildBot()
        {
            return new TestBotEnterprise.Bot(BotServices, ConversationState, UserState, TelemetryClient);
        }
    }
}
