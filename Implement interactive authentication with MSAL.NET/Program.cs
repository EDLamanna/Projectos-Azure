using Microsoft.Identity.Client;
using dotenv.net;

// Load environment variables from .env file
DotEnv.Load();
var envVars = DotEnv.Read();

// Retrieve Azure AD Application ID and tenant ID from environment variables
string _clientId = envVars["CLIENT_ID"];
string _tenantId = envVars["TENANT_ID"];

// ADD CODE TO DEFINE SCOPES AND CREATE CLIENT
    // Define the scopes required for authentication
    string[] _scopes = { "User.Read" };

    // Build the MSAL public client application with authority and redirect URI
    var app = PublicClientApplicationBuilder.Create(_clientId)
        .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
        .WithDefaultRedirectUri()
        .Build();

// ADD CODE TO ACQUIRE AN ACCESS TOKEN
    // Attempt to acquire an access token silently or interactively
        AuthenticationResult result;

        var accounts = await app.GetAccountsAsync();

        try
        {
            result = await app.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                            .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            result = await app.AcquireTokenWithDeviceCode(_scopes, deviceCodeResult =>
            {
                Console.WriteLine(deviceCodeResult.Message);
                return Task.CompletedTask;
            }).ExecuteAsync();
        }

        Console.WriteLine($"Access Token:\n{result.AccessToken}");