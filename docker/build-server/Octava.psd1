@{

    # Script module or binary module file associated with this manifest.
    RootModule = 'Octava.psm1'

    # Version number of this module.
    ModuleVersion = '1.0.0'

    # ID used to uniquely identify this module
    GUID = 'd7ba5c7c-3fef-410f-a8d2-19a78acbba1e'

    # Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
    FunctionsToExport = @('Get-DockerComposeFile')

    # List of all files packaged with this module
    FileList = @(
    '.\Octava.psd1','.\Octava.psm1','.\Public\Get-DockerComposeFile.ps1'
   )
}