using System;
using System.Collections.Generic;
using System.Linq;

using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using CallFirePlugin.Plugins.ProxyClasses;

using CallfireApiClient;
using CallfireApiClient.Api.Campaigns.Model;

namespace CallFirePlugin.Plugins
{
    public partial class SendTextBroadcast : BaseWorkflow
    {
        [Input("Advanced Find")]
        [RequiredArgument]
        public InArgument<string> AdvancedFind { get; set; }

        [Input("Message")]
        [RequiredArgument]
        public InArgument<string> Message { get; set; }

        [Input("Broadcast Name")]
        public InArgument<string> BroadcastName { get; set; }

        public CallfireClient CFClient { get; set; }

        public void SetClient(string username, string password) {
            this.CFClient = new CallfireClient(username, password);
        }

        protected override void ExecuteInternal(LocalWorkflowContext context)
        {
            string findName = AdvancedFind.Get(context.CodeActivityContext);
            string message = Message.Get(context.CodeActivityContext);
            string broadcastName = BroadcastName.Get(context.CodeActivityContext);
        }

        public void SendTexts(List<Person> personList, string message, string broadcastName)
        {
            List<CallfireApiClient.Api.CallsTexts.Model.TextRecipient> recipientList = personList.Select(x =>
                new CallfireApiClient.Api.CallsTexts.Model.TextRecipient()
                {
                    PhoneNumber = x.HomePhone,
                }).ToList();

            var broadcast = new TextBroadcast()
            {
                Name = broadcastName ?? "CRMRECRUIT Broadcast",
                Message = message,
                Recipients = recipientList,
            };
            var id = this.CFClient.TextBroadcastsApi.Create(broadcast, true);
        }
    }
}
