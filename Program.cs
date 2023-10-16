// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Samples.Common;
using Azure.ResourceManager.Compute;

namespace ListVirtualMachineImages
{
    public class Program
    {
        /**
         * List all virtual machine image publishers and
         * list all virtual machine images published by Canonical, Red Hat and
         * SUSE by browsing through locations, publishers, offers, SKUs and images.
         */
        private static ResourceIdentifier? _resourceGroupId = null;
        public static async Task RunSample(ArmClient client)
        {
            //=================================================================
            // List all virtual machine image publishers and
            // list all virtual machine images
            // published by Canonical, Red Hat and SUSE
            // by browsing through locations, publishers, offers, SKUs and images
            var subscription = await client.GetDefaultSubscriptionAsync();
            var publishers = subscription.GetVirtualMachineImagePublishers(AzureLocation.EastUS).ToList();
            Utilities.Log("US East data center: printing list of \n" +
                            "a) Publishers and\n" +
                            "b) Images published by Canonical, Red Hat and Suse");
            Utilities.Log("=======================================================");
            Utilities.Log("\n");
            foreach (var publisher in publishers)
            {
                Utilities.Log("Publish - " + publisher.Name);
                if(
                    StringComparer.OrdinalIgnoreCase.Equals(publisher.Name, "Canonical") ||
                    StringComparer.OrdinalIgnoreCase.Equals(publisher.Name, "Suse") ||
                    StringComparer.OrdinalIgnoreCase.Equals(publisher.Name, "RedHat")
                  )
                {
                    Utilities.Log("\n\n");
                    Utilities.Log("=======================================================");
                    Utilities.Log("Located " + publisher.Name);
                    Utilities.Log("=======================================================");
                    Utilities.Log("Printing entries as publisher/offer/sku/image/version");
                    var offers = subscription.GetVirtualMachineImageOffers(AzureLocation.EastUS,publisher.Name);
                    foreach (var offer in offers)
                    {
                        var skus = subscription.GetVirtualMachineImageSkus(AzureLocation.EastUS, publisher.Name,offer.Name);
                        foreach(var sku in skus)
                        {
                            var images = subscription.GetVirtualMachineImages(AzureLocation.EastUS,publisher.Name,offer.Name,sku.Name);
                            foreach(var image in images)
                            {
                                Utilities.Log($"Image - {publisher.Name}/{offer.Name}/{sku.Name}/{image.Name}");
                            }
                        }
                        Utilities.Log("\n\n");
                    }
                }
            }
        }
        public static async Task Main(string[] args)
        {
            try
            {
                var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
                var subscription = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
                ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                ArmClient client = new ArmClient(credential, subscription);
                await RunSample(client);
            }
            catch (Exception e)
            {
                Utilities.Log(e);
            }
        }
    }
}