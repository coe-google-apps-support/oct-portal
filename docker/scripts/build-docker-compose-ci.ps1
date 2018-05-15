#This PowerShell script runs on the Docker-Container-CI build agent, 
# built by Dockerfile.build-server

$BUILD_BUILDID = (Get-ChildItem Env:BUILD_BUILDID).Value
$BUILD_ARTIFACTSTAGINGDIRECTORY = (Get-ChildItem Env:BUILD_ARTIFACTSTAGINGDIRECTORY).Value
Write-Host "BuildId is $BUILD_BUILDID"

$od = Get-DockerComposeFile ./docker-compose.yml
$od.ReplaceImage("initiatives-vue", "coeoctava.azurecr.io/initiatives-vue:dev-1.0.$BUILD_BUILDID")
$od.ReplaceImage("initiatives-webapi", "coeoctava.azurecr.io/initiatives-webapi:v1.0.$BUILD_BUILDID")
$od.ReplaceImage("nginx", "coeoctava.azurecr.io/nginx:v1.0.$BUILD_BUILDID")

#We have a temporary collection because we can't change collections directly
$serviceNames = New-Object "System.Collections.Generic.List[string]" ($od.services.Keys.Count)

foreach ($svcName in $od.services.Keys)
{
  $svc = $od.services[$svcName]

  #Ensure unique names for the services
  $serviceNames.add($svcName)

  #Remove all exposed ports
  $svc.Remove("ports")

  #remove all volume, as they could interfere with multiple running instances
  $svc.Remove("volumes")  

  #remove the "restart" section if it exists, since if there are errors
  #we don't need to waste resources constantly restarting
  $svc.Remove("restart")
}

# Postfix _$(BuildId) to the services names to ensure uniqueness
foreach ($svcName in $serviceNames) {
  $svc = $od.services[$svcName];
  $od.services.Remove($svcName);
  $od.services[$svcName + "_$BUILD_BUILDID"] = $svc

  # same goes for the "depends_on" collection
  if ($svc.Contains("depends_on")) {
    $dependencies = $svc["depends_on"];
    #dependencies is an array of strings so we can do this a little easier
    for ($i=0; $i -lt $dependencies.length; $i++) {
      $dependencies[$i] = $dependencies[$i].ToString() + "_$BUILD_BUILDID"
    }
  }
}

#Add ports back somes images to allow us to map to a host port
$od.services.nginx["ports"] = @("${PORT}:80")
$od.services.'wordpress-db'["ports"] = @("${MYSQL_PORT}:3306")
$od.services.'initiatives-db'["ports"] = @("${MSSQL_PORT}:1433")

# Finally, we can remove the root "volumes" section since we won't have any left here
$od.Remove("volumes")

Write-Host "docker-compose:"
$od.ToString()

Write-Host "Saving docker-compose to $BUILD_ARTIFACTSTAGINGDIRECTORY"

$od.ToString() > $BUILD_ARTIFACTSTAGINGDIRECTORY/docker-compose.yml