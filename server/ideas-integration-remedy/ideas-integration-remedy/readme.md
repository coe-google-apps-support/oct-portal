This application listens for notifications on the service bus that initiatives have been created. WHhen they are created, new work orders are created in Remedy.

To regenerate the RemedyService, start a new "Developer Command Prompt for Visual Studio", and type in:
svcutil.exe /l:cs /t:code /n:*,CoE.Ideas.Remedy.RemedyServiceReference /o:Reference.cs http://coetaars1.coe.ads/arsys/WSDL/public/coetaars2/COE_WOI_WorkOrderInterface_WS_Octava