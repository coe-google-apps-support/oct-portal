using System;
using CoE.Issues.Core.Data;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;
using CoE.Ideas.Shared.People;

namespace CoE.Issues.Remedy
{
    public class RemedyService : IRemedyService
    {
        public RemedyService(HPD_IncidentInterface_Create_WSPortTypePortType remedyClient,
                   IOptions<RemedyServiceOptions> options,
                   Serilog.ILogger logger)
        {
            _remedyClient = remedyClient ?? throw new ArgumentNullException("remedyClient");
            if (options == null)
                throw new ArgumentNullException("options");
            _options = options?.Value;
            _logger = logger ?? throw new ArgumentNullException("logger");
        }

        private readonly HPD_IncidentInterface_Create_WSPortTypePortType _remedyClient;
        private readonly RemedyServiceOptions _options;
        private readonly Serilog.ILogger _logger;


        public virtual async Task<string> PostNewissueAsync(Issue issue, PersonData personData)
        {
            try
            {
                var request = new HelpDesk_Submit_ServiceRequest(
                    AuthenticationInfo: new AuthenticationInfo()
                    {
                        userName = _options.ServiceUserName,
                        password = _options.ServicePassword
                    },


                     Assigned_Group: null,
                     Assigned_Group_Shift_Name: null,
                     Assigned_Support_Company: null,
                     Assigned_Support_Organization: null,
                     Assignee: null,
                     Categorization_Tier_1: _options.CategorizationTier1,
                     Categorization_Tier_2: _options.CategorizationTier2,
                     Categorization_Tier_3: _options.CategorizationTier3,
                     CI_Name: null,
                     Closure_Manufacturer: null,
                     Closure_Product_Category_Tier1: null,
                     Closure_Product_Category_Tier2: null,
                     Closure_Product_Category_Tier3: null,
                     Closure_Product_Model_Version: null,
                     Closure_Product_Name: null,
                     Department: null,
                     First_Name: null,
                     Impact: null,
                     Last_Name: null,
                     Lookup_Keyword: null,
                     Manufacturer: null,
                     Product_Categorization_Tier_1: null,
                     Product_Categorization_Tier_2: null,
                     Product_Categorization_Tier_3: null,
                     Product_Model_Version: null,
                     Product_Name: null,
                     Reported_Source: null,
                     Resolution: null,
                     Resolution_Category_Tier_1: null,
                     Resolution_Category_Tier_2: null,
                     Resolution_Category_Tier_3: null,
                     Service_Type: null,
                     Status: StatusType.New,
                     Action: _options.Z1D_Action,
                     Create_Request: Create_RequestType.No,
                     Summary: issue.Title,
                     Notes: "Customer Name: " + personData.Surname + " " + personData.GivenName + '\n' +
                                 "Customer Email: " + personData.Email + '\n' +
                                 "Customer Phone Number: " + personData.Telephone + '\n' +
                                 "issue's Description: " + issue.Description,
                     Urgency: null,
                     Work_Info_Summary: null,
                     Work_Info_Notes: null,
                     Work_Info_Type: Work_Info_TypeType.General,
                     Work_Info_Date: DateTime.Now,
                     Work_Info_Source: Work_Info_SourceType.Other,
                     Work_Info_Locked: Create_RequestType.No,
                     Work_Info_View_Access: Work_Info_View_AccessType.Internal,
                     Middle_Initial: null,
                     Status_Reason: Status_ReasonType.Request,
                     Direct_Contact_First_Name: _options.CustomerFirstName,
                     Direct_Contact_Middle_Initial: null,
                     Direct_Contact_Last_Name: _options.CustomerLastName,
                     TemplateID: _options.TemplateId,
                     ServiceCI: null,
                     ServiceCI_ReconID: null,
                     HPD_CI: null,
                     HPD_CI_ReconID: null,
                     HPD_CI_FormName: null,
                     WorkInfoAttachment1Name: null,
                     WorkInfoAttachment1Data: null,
                     WorkInfoAttachment1OrigSize: 0,
                     Login_ID: _options.CustomerLoginId,
                     Customer_Company: _options.CustomerCompany,
                     Corporate_ID: null

);

                var response = await _remedyClient.HelpDesk_Submit_ServiceAsync(request);
                return response.Incident_Number;
            }
            catch (Exception err)
            {
                _logger.Error(err, "Unable to create incident in Remedy for issue { issueId }: { ErrorMessage} ", issue.Id, err.Message);
                throw;
            }
        }

    }
}


