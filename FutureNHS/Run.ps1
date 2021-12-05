


$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$solutionPath = (Join-Path $scriptPath 'FutureNHS.sln')
dotnet restore $scriptPath

dotnet build $solutionPath

$apiPath = (Join-Path $scriptPath 'FutureNHS.Api')
dotnet run --project $apiPath