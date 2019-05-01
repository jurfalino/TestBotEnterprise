using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace TestBotEnterprise.Middleware
{
    public class ResponseMiddleware : Microsoft.Bot.Builder.ITurnContext
    {
        public ResponseMiddleware()
        {

        }

        public BotAdapter Adapter => throw new NotImplementedException();

        public TurnContextStateCollection TurnState => throw new NotImplementedException();

        public Activity Activity => throw new NotImplementedException();

        public bool Responded => throw new NotImplementedException();

        public Task DeleteActivityAsync(string activityId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteActivityAsync(ConversationReference conversationReference, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ITurnContext OnDeleteActivity(DeleteActivityHandler handler)
        {
            throw new NotImplementedException();
        }

        public ITurnContext OnSendActivities(SendActivitiesHandler handler)
        {
            throw new NotImplementedException();
        }

        public ITurnContext OnUpdateActivity(UpdateActivityHandler handler)
        {
            throw new NotImplementedException();
        }

        public Task<ResourceResponse[]> SendActivitiesAsync(IActivity[] activities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task SendActivitiesHandler(ITurnContext turnContext, List<Activity> activities, Func<Task<ResourceResponse[]>> next)
        {
        }

        public Task<ResourceResponse> SendActivityAsync(string textReplyToSend, string speak = null, string inputHint = "acceptingInput", CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ResourceResponse> SendActivityAsync(IActivity activity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ResourceResponse> UpdateActivityAsync(IActivity activity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
