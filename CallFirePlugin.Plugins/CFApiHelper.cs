using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;

using CallfireApiClient;
using CallfireApiClient.Api.Campaigns.Model;

using CallFirePlugin.Plugins.ProxyClasses;

namespace CallFirePlugin.Plugins
{
    public class CFApiHelper
    {
        public static List<Person> GetPeopleFromFetch(
                   OrganizationServiceContext organizationServiceContext,
                   IOrganizationService organizationService,
                   string findName)
        {
            // Find the correct Fetch XML
            var findResults = organizationServiceContext.CreateQuery<SavedView>()
                .Where(x => x.Name == findName).ToList();

            if (findResults.Count() != 1)
            {
                throw new Exception(
                    $"Expected exactly one Advanced Find name \"{findName}\", but found {findResults.Count()}");
            }

            SavedView find = findResults.First();

            // Create the request
            ExecuteByIdSavedQueryRequest executeSavedQueryRequest = new ExecuteByIdSavedQueryRequest()
            {
                EntityId = find.Id
            };

            // Execute the request
            var results = organizationService.RetrieveMultiple(new FetchExpression(find.FetchXML))
                .ToProxies<Person>();

            return results;
        }

        public static void SendTextBroadcast(CallfireClient cfClient, List<Person> personList, string message)
        {
            List<CallfireApiClient.Api.CallsTexts.Model.TextRecipient> recipientList = personList.Select(x =>
                new CallfireApiClient.Api.CallsTexts.Model.TextRecipient()
                {
                    PhoneNumber = x.HomePhone,
                }).ToList();

            var broadcast = new TextBroadcast()
            {
                Name = "CRMRECRUIT Broadcast",
                Message = message,
                Recipients = recipientList,
            };
            var id = cfClient.TextBroadcastsApi.Create(broadcast, true);
        }
    }
}
