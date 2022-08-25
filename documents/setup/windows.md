
# Windows Setup - FutureNHS

## Install Git
[Git setup instructions](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)
[Git download page](https://git-scm.com/download/win)

## Uninstall Node (If you don’t already have NVM installed)
(I'm sorry)

[Node Removal instructions](https://stackoverflow.com/questions/20711240/how-to-completely-remove-node-js-from-windows)

## Install NVM
[NVM setup instructions](https://docs.microsoft.com/en-us/windows/dev-environment/javascript/nodejs-on-windows)

### Install version 14.18.3 of Node

```
nvm install v14.18.3
nvm use v14.18.3
nvm alias default v14.18.3
```

## Install Microsft Sql 2019
[MS Sql setup instructions](https://www.microsoft.com/en-gb/evalcenter/evaluate-sql-server-2019)

Use the default settings, make a note of the sa password, you will need this for later, you can alternatively create a separate user and use those credentials to connect, these details will need to be added to the shared secrets at the end.

### Install MS Sql Management studio
[MS Sql setup instructions](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)

## Install SqlPackage
[SqlPackage setup instructions](https://docs.microsoft.com/en-us/sql/tools/sqlpackage/sqlpackage-download?view=sql-server-ver16)

In addition to this also add it to your path in the windows environment variables.

**To add a directory to the existing PATH in Windows**

1. Launch "Control Panel"
2. "System"
3. "Advanced system settings"
4. Switch to "Advanced" tab
5. "Environment variables"
6. Under "System Variables" (for all users), select "Path"
7. "Edit"
8. "New"
9. C:\Program Files\Microsoft SQL Server\160\DAC\bin
10. "Ok"

## Install .Net 6.0 sdk
[.Net 6 setup instructions](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Install gulp

[Gulp setup instructions](https://gulpjs.com/docs/en/)

```
npm install --global gulp-cli
```

## Git Clone Repo
cd to location you want to pull code down to, eg cd Documents/Source
```
git clone https://github.com/tim-hunt/futurenhs.git
cd futurenhs
git reset --hard origin/SPRINT
```

### Pull down packages for root and web app
```
npm i
cd futurenhs.app
npm i
```

## Config 
Save the config from below (Api Secrets) into an apisecrets.json file and fill in all of the details with the relevant connections strings and credentials, make a note of the location for this file and run the following command:
cd ..

```
type .\apisecrets.json | dotnet user-secrets set --project "futurenhs.api/FutureNHS.Api/FutureNHS.Api.csproj"
```

Save the config from below (Umbraco Api Secrets) into an umbracosecrets.json file and fill in all of the details with the relevant connections strings and credentials, make a note of the location for this file and run the following command:

```
type .\umbracosecrets.json | dotnet user-secrets set --project "futurenhs.content.api/Umbraco9ContentApi.Umbraco/Umbraco9ContentApi.Umbraco.csproj"
```

Save the config from below  (Web app secrets) into a file called .env.local in /futurenhs.app

## Build front end first time
```
cd futurenhs.app

npm run build
```

## Run the site

```
cd ..

gulp activate
```

# Config Secrets

## Api Secrets:
```
{
 "AzureImageBlobStorage": "UseDevelopmentStorage=true",

 "AzureImageBlobStorage:queue": "UseDevelopmentStorage=true",

 "AzureImageBlobStorage:blob": "UseDevelopmentStorage=true",

 "AzurePlatform:AzureFileBlobStorage:ConnectionString": "<BlobConnectionstringHere>"

 "AzurePlatform:AzureFileBlobStorage:PrimaryServiceUrl": "<ServiceUrlHere>",

 "AzurePlatform:AzureFileBlobStorage:GeoRedundantServiceUrl": "<ServiceUrlHere>",

 "AzurePlatform:AzureFileBlobStorage:ContainerName": "files",

 "AzurePlatform:AzureImageBlobStorage:ConnectionString": "<BlobConnectionstringHere>",

 "AzurePlatform:AzureImageBlobStorage:PrimaryServiceUrl": "<ServiceUrlHere>",

 "AzurePlatform:AzureImageBlobStorage:GeoRedundantServiceUrl": "<ServiceUrlHere>",

 "AzurePlatform:AzureImageBlobStorage:ContainerName": "images",

 "AzurePlatform:AzureSql:ReadWriteConnectionString": "SqlConnectionStringHere>",

 "AzurePlatform:AzureSql:ReadOnlyConnectionString": "<ReadOnlySqlConnectionStringHere>",

 "AzureBlobStorage:FilePrimaryConnectionString": "<BlobConnectionstringHere>",

 "AzureBlobStorage:ImagePrimaryConnectionString": "<BlobConnectionstringHere>",

 "SharedSecrets:WebApplication": "<StrongUniqueStringHere>",

 "SharedSecrets:Owner": "FutureNHS",

 "GovNotify:ApiKey": "<keyhere>"
}
```

## Umbraco Api Secrets
```
{
  "APPINSIGHTS_CONNECTIONSTRING": "InstrumentationKey=keyhere",

  "ConnectionStrings:umbracoDbDSN": "SqlConnectionStringHere>",

  "Umbraco:Storage:AzureBlob:Media:ConnectionString": "<BlobConnectionstringHere>",

  "Umbraco:Storage:AzureBlob:Media:ContainerName": "content",

  "Logging:LogLevel:Default": "Error",

  "Logging:LogLevel:Microsoft": "Warning",

  "Logging:LogLevel:Microsoft.Hosting.Lifetime": "Information",

  "Logging:TableStorageConfiguration:ConnectionString": "<BlobConnectionstringHere>",

  "Logging:TableStorageConfiguration:TableName": "Logs"
}
```

## Web app secrets

```
PORT = 5000
APP_URL = http://localhost:$PORT
NEXT_PUBLIC_GTM_KEY
NEXT_PUBLIC_ASSET_PREFIX
NEXT_PUBLIC_API_GATEWAY_BASE_URL = $APP_URL/api/gateway
NEXT_PUBLIC_API_BASE_URL = http://localhost:9999/api
MVC_FORUM_HEALTH_CHECK_URL = http://localhost:8888/api/healthcheck/heartbeat
API_HEALTH_CHECK_URL = http://localhost:9999/health-check
COOKIE_PARSER_SECRET = <StrongUniqueStringHere>
SHAREDSECRETS_APIAPPLICATION = <StrongStringHereFromApiSharedSecret>
NEXTAUTH_URL = $APP_URL
NEXTAUTH_SECRET = <StrongUniqueStringHere>
AZURE_AD_B2C_TENANT_NAME = <B2CTenantName>
AZURE_AD_B2C_CLIENT_ID = <B2CClientIdHere>
AZURE_AD_B2C_CLIENT_SECRET = <B2CClientSecret>
AZURE_AD_B2C_PRIMARY_USER_FLOW = <B2CUserFlow>
APPINSIGHTS_INSTRUMENTATIONKEY = PLACEHOLDER
```

