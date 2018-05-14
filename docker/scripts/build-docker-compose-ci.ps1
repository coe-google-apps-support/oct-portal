#This PowerShell script runs on the Docker-Container-CI build agent, 
# built by Dockerfile.build-server

#Import the PSYaml module, installed on agent by the Dockerfile.build-server

Import-Module /var/PSYaml/
function New-DockerCompose {
  param (
      [string]$BaseDockerComposePath,
      [string]$version
  )
}

$dockerCompose = Get-Content /vsts/agent/_work/1/s/docker-compose.yml | Out-String | ConvertFrom-Yaml


$yaml.services.'initiatives-vue'.image = "coeoctava.azurecr.io/initiatives-vue:dev-1.0.35"
