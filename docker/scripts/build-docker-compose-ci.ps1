#This PowerShell script runs on the Docker-Container-CI build agent, 
# built by Dockerfile.build-server

Write-Host "BuildId is $BUILD_BUILDID"
Write-Host "Staging Directory is $BUILD_ARTIFACTSTAGINGDIRECTORY"

$od = Get-DockerComposeFile ./docker-compose.yml
$od.ReplaceImage("initiatives-vue", "coeoctava.azurecr.io/initiatives-vue:dev-1.0.$BUILD_BUILDID")
$od.ReplaceImage("initiatives-webapi", "coeoctava.azurecr.io/initiatives-webapi:v1.0.$BUILD_BUILDID")
$od.ReplaceImage("nginx", "coeoctava.azurecr.io/nginx:v1.0.$BUILD_BUILDID")

foreach ($svcName in $od.services.Keys)
{
  $svc = $od.services[$svcName];

  #Remove all exposed ports
  $svc.Remove("ports");

  #Ensure unique names for the services
  $od.services.Remove($svcName);
  $od.services[$svcName + "_$BUILD_BUILDID"] = $svc

  if ($svc.Contains("depends_on")) {
    $dependencies = $svc["depends_on"];
    foreach ($dependencyName in $dependencies.Keys) {
      $dependency = $dependencies[$dependencyName];
      $dependencies.Remove($dependency);
      $dependencies[$dependencyName + "_$BUILD_BUILDID"] = $dependency;
    }
  }
}

$od > ./docker-compose.yml