using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace pds
{
    class Program
    {
        private const string _tenantId = "<tenant guid>";
        private const string _daemonClientId = "<client id of the daemon app registration - guid>";
        private const string _daemonAppSecret = "<secret of daemon app registration>";

        private const string _endPointBaseUrl = "https://pdauth1.azurewebsites.net";
        private const string _path = "api/values";

        private readonly static string[] _scopes = new string[] {
            $"{_endPointBaseUrl}/.default",
        };        
                
        private readonly static string _authority = $"https://login.microsoftonline.com/{_tenantId}";

        static async Task Main(string[] args)
        {
            await TestMSAL(args);
        }

        // Based on this: https://github.com/Azure-Samples/active-directory-dotnetcore-devicecodeflow-v2/blob/master/device-code-flow-console/PublicAppUsingDeviceCodeFlow.cs
        static async Task TestMSAL(string[] args)
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(_daemonClientId)
                .WithClientSecret(_daemonAppSecret)
                .WithAuthority(new Uri(_authority))
                .Build();

            AuthenticationResult authResult = null;

            // See also https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-daemon-acquire-token?tabs=dotnet#acquiretokenforclient-api
            try
            {
                authResult = await app.AcquireTokenForClient(_scopes).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // The application doesn't have sufficient permissions.
                // - Did you declare enough app permissions during app creation?
                // - Did the tenant admin grant permissions to the application?
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
                // Mitigation: Change the scope to be as expected.
            }

            using var httpClient = new HttpClient();
            using var httpRequest = new HttpRequestMessage();

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            httpRequest.RequestUri = new Uri($"{_endPointBaseUrl}/{_path}");
            
            var response = await httpClient.SendAsync(httpRequest);

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"\r\n Response: {result}");

            Console.WriteLine("\r\nPress any key....");
            Console.ReadKey();            
        }
    }
}
