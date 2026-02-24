using Microsoft.Graph;
using Azure.Identity;
using dotenv.net;

// Load environment variables from .env file
DotEnv.Load();
var envVars = DotEnv.Read();

// Retrieve Azure AD Application ID and tenant ID from environment variables
string clientId = envVars["CLIENT_ID"];
string tenantId = envVars["TENANT_ID"];

// Validate that required environment variables are set
if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(tenantId))
{
    Console.WriteLine("Please set CLIENT_ID and TENANT_ID environment variables.");
    return;
}

// Define the scopes required for Microsoft Graph
var scopes = new[] { "User.Read" };

// Configure device code authentication (works in Codespaces)
var credential = new DeviceCodeCredential(deviceCodeCallback: (code, cancellationToken) =>
{
    Console.WriteLine(code.Message);
    return Task.CompletedTask;
},
clientId: clientId,
tenantId: tenantId);

// Create Graph client with the credential
var graphClient = new GraphServiceClient(credential);

// Retrieve and display the user's profile information
Console.WriteLine("Retrieving user profile...");
await GetUserProfile(graphClient);

// Function to get and print the signed-in user's profile
async Task GetUserProfile(GraphServiceClient graphClient)
{
    try
    {
        // Call Microsoft Graph /me endpoint to get user info
        var me = await graphClient.Me.GetAsync();
        Console.WriteLine($"Display Name: {me?.DisplayName}");
        Console.WriteLine($"Principal Name: {me?.UserPrincipalName}");
        Console.WriteLine($"User Id: {me?.Id}");
    }
    catch (Exception ex)
    {
        // Print any errors encountered during the call
        Console.WriteLine($"Error retrieving profile: {ex.Message}");
    }
}