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
  $svc = $od.services[$svcName];

  #Remove all exposed ports
  $svc.Remove("ports");

  #Ensure unique names for the services
  $serviceNames.add($svcName);
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

Write-Host "docker-compose:"
$od.ToString()

Write-Host "Saving docker-compose to $BUILD_ARTIFACTSTAGINGDIRECTORY"

$od.ToString() > $BUILD_ARTIFACTSTAGINGDIRECTORY/docker-compose.yml