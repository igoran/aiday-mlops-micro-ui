using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.StaticFiles;
using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.Storage;
using Pulumi.Azure.Storage.Inputs;

class MyStack : Stack
{
    public string ProjectStack { get; }
    public string StackSuffix { get; }

    public MyStack()
    {
        ProjectStack = Deployment.Instance.ProjectName + "-" + Deployment.Instance.StackName;
        StackSuffix = Regex.Replace(Deployment.Instance.StackName, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase);

        var resourceGroup = new ResourceGroup(ProjectStack);

        var storageAccount = new Account("sa" + StackSuffix.ToLowerInvariant(), new AccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            EnableHttpsTrafficOnly = true,
            AccountReplicationType = "LRS",
            AccountTier = "Standard",
            AccountKind = "StorageV2",
            StaticWebsite = new AccountStaticWebsiteArgs
            {
                IndexDocument = "index.html"
            }
        });

        var wwwroot = new DirectoryInfo("../wwwroot");

        foreach (var file in wwwroot.GetFiles())
        {
            _ = new Blob(file.Name, new BlobArgs
            {
                Name = file.Name,
                StorageAccountName = storageAccount.Name,
                StorageContainerName = "$web",
                Type = "Block",
                Source = new FileAsset(file.FullName),
                ContentType = ResolveContentType(file.Name),
            });
        }

        // Export the Web address string for the storage account
        WebStaticEndpoint = storageAccount.PrimaryWebEndpoint;
    }

    [Output] public Output<string> WebStaticEndpoint { get; set; }


    private static string ResolveContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out string contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
}