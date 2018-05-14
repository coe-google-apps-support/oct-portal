function Get-DockerComposeFile {

    <#
    .SYNOPSIS
    Creates a new docker-compose.yml file from an existing one

    .DESCRIPTION
    Creates a new docker-compose.yml file from an existing one, replacing tags of specified images
    #>
    [CmdletBinding()]
    param (
        [parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $false)]
        [string]$Path,
        [parameter(Position = 1, Mandatory = $false, ValueFromPipeline = $true)]
        [hashtable]$ImageOverrides
    )
    begin { }
    process {
      #Output of ConvertFrom-Yaml is an OrderedDictionary
      $od = Get-Content $Path | Out-String | ConvertFrom-Yaml

      #Someday I may replace this with an actual .Net type in a compiled library
      $od | Add-Member -MemberType ScriptMethod -Name ToString -Value { $this | ConvertTo-Yaml } -Force
      $od | Add-Member -MemberType ScriptMethod -Name ReplaceImage -Value {
        param([string]$OldValue, [string]$NewValue)
        foreach ($k in $this.services.Keys) { 
          $svc = $this.services[$k]
          if ($svc.image -match $OldValue) {
            Write-Verbose ("Replacing " + $svc.image  + " with " + $NewValue)
            $svc.image = $NewValue
          }
        }
      }

      #return the $od object
      $od
    }
    end { }
  }