
# Windows Setup - FutureNHS

Load terminal and run the following commands

```
Sudo apt-get install git
```

## Install homebrew
[Brew instructions](https://brew.sh/)

```
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)"
```

❗️Restart terminal
## Uninstall Node (If you don’t already have NVM installed)

```
brew uninstall node
```

## Install NVM

```
curl https://raw.githubusercontent.com/creationix/nvm/master/install.sh |  shellenv
```
❗️Restart terminal

## Install specific version of Node - 14.18.3

```
nvm install v14.18.3
nvm use v14.18.3
nvm alias default v14.18.3
```

## Install Docker

[Docker setup instructions](https://docs.docker.com/desktop/install/linux-install/)

## Install gulp

[Gulp setup instructions](https://gulpjs.com/docs/en/)

```
npm install --global gulp-cli
```

## Install MSSQL

```
sudo docker pull mcr.microsoft.com/mssql/server:2022-latest
sudo docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=9um#Qu&6d3t5" -p 1433:1433 --name sql1 --hostname sql1  -d mcr.microsoft.com/mssql/server:2022-latest
```

## Install Azure data studio

[Azure data studio setup instructions](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio?view=sql-server-ver16)

## Install SQL Package
[SQL Package instructions](https://docs.microsoft.com/en-us/sql/tools/sqlpackage/sqlpackage-download?view=sql-server-ver16)

❗️Sql Package uses an old version of libssl so need to install the old one.
```
wget http://security.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2.16_amd64.deb 
wget http://security.ubuntu.com/ubuntu/pool/main/o/openssl/openssl_1.1.1f-1ubuntu2.16_amd64.deb 

sudo dpkg -i libssl1.1_1.1.1f-1ubuntu2.16_amd64.deb 
sudo dpkg -i openssl_1.1.1f-1ubuntu2.16_amd64.deb
```

slight tweak required to path as does not work in gulp, only terminal, when following the instructions from microsoft
```
mkdir sqlpackage
unzip ~/Downloads/sqlpackage-linux-x64-en-<Version>.zip -d ~/sqlpackage 
chmod a+x ~/sqlpackage/sqlpackage
echo 'export PATH="$HOME/sqlpackage:$PATH"' >> ~/.bash_profile
source ~/.bash_profile
sqlpackage
```

## Install .net 6
```
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```

### Install the SDK
```
sudo apt-get update && \ sudo apt-get install -y dotnet-sdk-6.0
```

## Git Clone Repo
cd to location you want to pull code down to, eg cd Documents/Source
```
git clone https://github.com/tim-hunt/futurenhs.git
cd futurenhs
git reset --hard origin/SPRINT
```

## Install npm packages
```
eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)"

brew install vips
```

### Install yarn
```
curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add -
echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list
sudo apt update
sudo apt install yarn
```

### Pull down packages for root and web app
```
yarn
cd futurenhs.app
yarn
```

## Config 
Save the config from below (Api Secrets) into an apisecrets.json file to your user’s drive and fill in all of the details with the relevant connections strings and credentials, make a note of the location for this file and run the following command:
cd ..

```
cat ~/apisecrets.json | dotnet user-secrets set --project "futurenhs.api/FutureNHS.Api/FutureNHS.Api.csproj"
```

Save the config from below (Umbraco Api Secrets) into an umbracosecrets.json file to your user’s drive and fill in all of the details with the relevant connections strings and credentials, make a note of the location for this file and run the following command:

```
cat ~/umbracosecrets.json | dotnet user-secrets set --project "futurenhs.content.api/Umbraco9ContentApi.Umbraco/Umbraco9ContentApi.Umbraco.csproj"
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

#Config Secrets

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

