using System;
using System.Collections.Generic;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using CallFirePlugin.Plugins.ProxyClasses;
using CallFirePlugin.Plugins;
using NUnit.Framework;
using CallfireApiClient;
using CallfireApiClient.Api.CallsTexts.Model;
using System.Linq;
using CallfireApiClient.Api.Campaigns.Model;

namespace CallfirePlugin.Plugins.Tests
{
    [TestFixture]
    public class CFApiHelperTests
    {
        [Test]
        public void Should_Retrieve_Records_By_Given_Find()
        {
            // Arrange
            var sampleView = new SavedView()
            {
                Name = "Sample",
                Id = Guid.NewGuid(),
                FetchXML = @"
<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
  <entity name=""contact"">
    <attribute name=""emailaddress1"" />
    <attribute name=""lastname"" />
    <attribute name=""firstname"" />
    <attribute name=""address1_telephone1"" />
    <attribute name=""contactid"" />
  </entity>
</fetch>",
            };
            var person = new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Billy",
                LastName = "Bob"
            };

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity>() {
                person,
                sampleView,
            });

            var service = fakedContext.GetOrganizationService();
            var serviceContext = new Microsoft.Xrm.Sdk.Client.OrganizationServiceContext(service);

            // Act
            var helper = new CFApiHelper();
            var results = CFApiHelper.GetPeopleFromFetch(
                serviceContext,
                service,
                sampleView.Name);

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(results.FirstOrDefault().Id, person.Id);
        }
        [Test]
        public void Should_Throw_If_Wrong_Count_of_Finds_Found()
        {
            var fakedContext = new XrmFakedContext();
            // Initialize Empty CRM
            fakedContext.Initialize(new List<Entity>());

            var service = fakedContext.GetOrganizationService();
            var serviceContext = new Microsoft.Xrm.Sdk.Client.OrganizationServiceContext(service);

            // Act
            var helper = new CFApiHelper();
            var exception = Assert.Throws<Exception>(() =>
                CFApiHelper.GetPeopleFromFetch(serviceContext, service, "foo"));
            Assert.That(exception.Message, Is.EqualTo("Expected exactly one Advanced Find name \"foo\", but found 0"));
        }

        [Test]
        public void Should_Send_Text_Broadcast()
        {
            var plugin = new SendTextBroadcast();
            plugin.SetClient("username", "password");

            plugin.SendTexts(
                new List<Person>() {
                    new Person { HomePhone = "7575754943" },
                    new Person{ HomePhone = "17577055492" },
                },
                "Hello wolrd! I'm trying out the new texting integration!",
                null);
        }

        [Test]
        public void SendVM()
        {
            var client = new CallfireClient("username", "password");
            var user = client.MeApi.GetAccount();

            var recipient1 = new CallRecipient { PhoneNumber = "17577055492", LiveMessage = "testMessage" };
            var recipients = new List<CallRecipient> { recipient1 };

            IList<Call> calls = client.CallsApi.Send(recipients, null, "items(id,fromNumber,state)");
        }
    }
}
