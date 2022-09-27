
# Mac OS - FutureNHS

Load terminal and run the following commands

## Install homebrew
[Brew instructions](https://brew.sh/)
```
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```
❗️Restart terminal

## Uninstall Node (If you don’t already have NVM installed)

```
brew uninstall node
```

## Install NVM

```
curl https://raw.githubusercontent.com/creationix/nvm/master/install.sh | bash
```
❗️Restart terminal

## Install specific version of Node - 14.18.3

```
nvm install v14.18.3
nvm use v14.18.3
nvm alias default v14.18.3
```

## Install Docker

[Docker setup instructions](https://docs.docker.com/desktop/install/mac-install/)

## Install Git
[Git setup instructions](https://git-scm.com/download/mac)

```
brew install git
```

## Install gulp
[Gulp setup instructions](https://gulpjs.com/docs/en/)

```
npm install --global gulp-cli
```

## Install MSSql
[MSSql setup instructions](https://database.guide/how-to-install-sql-server-on-an-m1-mac-arm64/)

```
sudo docker pull mcr.microsoft.com/azure-sql-edge

sudo docker run --cap-add SYS_PTRACE -e 'ACCEPT_EULA=1' -e 'MSSQL_SA_PASSWORD=<ENTERSTRONGPASSWORDHERE>' -p 1433:1433 --name sqledge -d mcr.microsoft.com/azure-sql-edge
```

## Install Azure data studio

[Azure data studio setup instructions](https://database.guide/how-to-install-azure-data-studio-on-a-mac/)

## Install SQL Package
[SQL Package instructions](https://docs.microsoft.com/en-us/sql/tools/sqlpackage/sqlpackage-download?view=sql-server-ver16)


slight tweak required to path as does not work in gulp, only terminal, when following the instructions from microsoft
```
mkdir sqlpackage
unzip ~/Downloads/sqlpackage-osx-<version string>.zip -d /usr/local/share/sqlpackage
chmod +x /usr/local/share/sqlpackage/sqlpackage
echo 'export PATH="/usr/local/share/sqlpackage:$PATH"' >> ~/.bash_profile
source ~/.bash_profile
sqlpackage
```

## Install .net 6
[.net 6 instructions](https://docs.microsoft.com/en-us/dotnet/core/install/macos)

For M1 chips select Arm64 version otherwise pick x64

## Git Clone Repo
cd to location you want to pull code down to, eg cd Documents/Source
```
git clone https://github.com/nhsengland/futurenhs.git
cd futurenhs
git reset --hard origin/main
```

## Ensure python is in the env path
```
brew install pyenv

pyenv install 3.10.3

pyenv global 3.10.3

echo "export PATH=\"\${HOME}/.pyenv/shims:\${PATH}\"" >> ~/.bash_profile
```
### open a new terminal window and confirm your pyenv version is mapped to python
```
which python

python --version
```

## M1 Chip issues
Some issues were found running on m1 chip, following the guidance [here](https://github.com/nuxt/image/issues/204) to fix them 

### Install gcc
The "libvps" depends on gcc, so do:
```
brew install --build-from-source gcc
```
Install XCode Build Tools CLI
Also required by "libvps"
```
xcode-select --install
```

### Install "vips"
```
brew install vips
```


## Pull down packages for root and web app
```
npm i
cd futurenhs.app
npm i
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

## Run the app

```
cd futurenhs.app

npm run start:dev 
```
### Tip:

 - Allow keychain popups
 - Make sure your AirPlay receiver [System Preferences -> Sharing] is ticked off to listen port 5000

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
  "APPINSIGHTS_CONNECTIONSTRING": "<keyhere>",

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

