using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;


var builder = WebApplication.CreateBuilder(args);
//ConnectAppconfigurationfromLocalhost();
ConnectAppconfigurationFromAzure();

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Run Error: " + ex.Message);
    throw;
}

void ConnectAppconfigurationfromLocalhost()
{
    //Run from local host
    // Needed Authorizations when using AzureCliCredential:
    // 1. Add role assignment " App Configuration Data Reader" on App Configuration to the user account.
    // 2. Enable system assigned identity in app configuration and add the role assignment "Key Vault Secrets User" in keyvault 


    //Needed Authorizations when using DefaultClientCredential
    // 1.Enable system assigned identity in app configuration and add the role assignment "Key Vault Secrets User" in keyvault 
    try
    {
        AzureCliCredential credential = new AzureCliCredential();

        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            // we can also use connection string with access keys for the appconfiguration
            options.Connect(new Uri("https://kalibrate-appconfiguration.azconfig.io"), credential)
                   //.Select("*") // Loads key without any Label
                   .Select("*", labelFilter: "App1")       // Load keys with label "App2"
                   .Select("*", labelFilter: LabelFilter.Null) // Load keys without any label
                   .ConfigureKeyVault(kv =>
                   {
                       kv.SetCredential(credential);
                   });
        });

    }
    catch (Exception ex)
    {
        Console.WriteLine("App Configuration Error: " + ex.Message);
        throw;
    }
}

void ConnectAppconfigurationFromAzure()
{

    /*After deploying to Azure App Service
    Needed Authorizations when using user assigned managed identity:
    1. Create user assigned managed identity 1 and add it under Web App.
    2. Add role assignments into App Configuration with managed idenitity 1 as "App Configuration Data Reader" - for webapp to appconfiguration
    3. Create user assigned managed identity 2 and add it under app configuration.
    4. Add role assignment into Key Vault with managed identity 2 as "Key vault Secrets User"  - for appconfiguration to keyvault
    5. Add role assignmets into Key Vault with managed identity 1 as "Key vault Secrets User" - for webapp to keyvault


    After deploying to Azure App Service 
    Needed Authorizations when using system assigned managed identity:
    1. Enable system assigned managed identity(1) in web app
    2. Add role assignments into App Configuration with webapp's system assigned managed idenitity (1) as "App Configuration Data Reader" - for webapp to appconfiguration
    3. Enable system assigned managed identity (2) under app configuration.
    4. Add role assignment into Key Vault with managed identity 2 as "Key vault Secrets User"  - for appconfiguration to keyvault
    5. Add role assignmets into Key Vault with managed identity 1 as "Key vault Secrets User" - for webapp to keyvault
    */

    #region Azure Run
    try
    {

        // when running in Azure.
        var credentialOptions_ApptoAppconfig = new DefaultAzureCredentialOptions
        {
            ManagedIdentityClientId = "274302c3-a0e5-47ea-af90-739c9311af4a"
        };
        var credential_ApptoAppConfig = new DefaultAzureCredential(credentialOptions_ApptoAppconfig);

        // when running in Azure.
        var credentialOptions_Appconfigtokv = new DefaultAzureCredentialOptions
        {
            ManagedIdentityClientId = "c28737b8-a4a3-4a95-a35e-a377a26c1d83"
        };
        var credential_Appconfigtokv = new DefaultAzureCredential(credentialOptions_ApptoAppconfig);


        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            //options.Connect(@"Endpoint=https://kalibrate-appconfiguration.azconfig.io;Id=75me;Secret=yWhWjymdgLKCys0eb26TtKHnu401BPXNOirFAG3Sm9rFWaQH79ZsJQQJ99BFACHYHv6Y4SklAAACAZAC3SsQ")
            options.Connect(new Uri("https://kalibrate-appconfiguration.azconfig.io"), credential_ApptoAppConfig)
              .Select("*", labelFilter: "App1")// Optional: apply label filter
               .Select("*", labelFilter: LabelFilter.Null) // Load keys without any label
               .ConfigureKeyVault(kv =>
               {
                   kv.SetCredential(credential_Appconfigtokv);
               });
        });

    }
    catch (Exception ex)
    {
        Console.WriteLine("App Configuration Error: " + ex.Message);
        throw;
    }
    #endregion
}