This application reads changes from the Remedy application and posts notification on the service bus

To regenerate the RemedyService, start a new "Developer Command Prompt for Visual Studio", cd to the Connected Service directory, and type in:

`svcutil.exe /l:cs /t:code /n:*,CoE.Ideas.Remedy.Watcher.RemedyServiceReference /o:Reference.cs http://coetaars1.coe.ads/arsys/WSDL/public/coetaars2/COE_WOI_WorkOrder_Search_Octava`
