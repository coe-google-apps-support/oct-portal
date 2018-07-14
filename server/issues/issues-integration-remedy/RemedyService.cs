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
                    Customer_Company: _options.CustomerCompany,
                    Login_ID: _options.CustomerLoginId,
                    Action: _options.Z1D_Action,
                    Summary: issue.Title,
                    Notes: "Customer Name: " + personData.Surname + " " + personData.GivenName + '\n' +
                                 "Customer Email: " + personData.Email + '\n' +
                                 "Customer Phone Number: " + personData.Telephone + '\n' +
                                 "issue's Description: " + issue.Description,
                                
                    Work_Info_Type: Work_Info_TypeType.General,
                    Customer_Company: _options.LocationCompany,
                    Categorization_Tier_1: _options.CategorizationTier1,
                    Categorization_Tier_2: _options.CategorizationTier2,
                    Categorization_Tier_3: _options.CategorizationTier3,
                    TemplateID: _options.TemplateId,
                    Direct_Contact_First_Name: _options.CustomerFirstName,
                    Direct_Contact_Last_Name: _options.CustomerLastName);

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


                    //string assigned_group, 
                    //string assigned_group_shift_name, 
                    //string assigned_support_company, 
                    //string assigned_support_organization, 
                    //string assignee, 
                    //string categorization_tier_1, 
                    //string categorization_tier_2, 
                    //string categorization_tier_3, 
                    //string ci_name, 
                    //string closure_manufacturer, 
                    //string closure_product_category_tier1, 
                    //string closure_product_category_tier2, 
                    //string closure_product_category_tier3, 
                    //string closure_product_model_version, 
                    //string closure_product_name, 
                    //string department, 
                    //string first_name, 
                    //system.nullable<impacttype> impact, 
                    //string last_name, 
                    //string lookup_keyword, 
                    //string manufacturer, 
                    //string product_categorization_tier_1, 
                    //string product_categorization_tier_2, 
                    //string product_categorization_tier_3, 
                    //string product_model_version, 
                    //string product_name, 
                    //system.nullable<reported_sourcetype> reported_source, 
                    //string resolution, 
                    //string resolution_category_tier_1, 
                    //string resolution_category_tier_2, 
                    //string resolution_category_tier_3, 
                    //system.nullable<service_typetype> service_type, 
                    //coe.issues.remedy.watcher.remedyservicereference.statustype status, 
                    //string action, 
                    //coe.issues.remedy.watcher.remedyservicereference.create_requesttype create_request, 
                    //string summary, 
                    //string notes, 
                    //system.nullable<urgencytype> urgency, 
                    //string work_info_summary, 
                    //string work_info_notes, 
                    //coe.issues.remedy.watcher.remedyservicereference.work_info_typetype work_info_type, 
                    //system.datetime work_info_date, 
                    //coe.issues.remedy.watcher.remedyservicereference.work_info_sourcetype work_info_source, 
                    //coe.issues.remedy.watcher.remedyservicereference.create_requesttype work_info_locked, 
                    //coe.issues.remedy.watcher.remedyservicereference.work_info_view_accesstype work_info_view_access, 
                    //string middle_initial, 
                    //coe.issues.remedy.watcher.remedyservicereference.status_reasontype status_reason, 
                    //string direct_contact_first_name, 
                    //string direct_contact_middle_initial, 
                    //string direct_contact_last_name, 
                    //string templateid, 
                    //string serviceci, 
                    //string serviceci_reconid, 
                    //string hpd_ci, 
                    //string hpd_ci_reconid, 
                    //string hpd_ci_formname, 
                    //string workinfoattachment1name, 
                    //byte[] workinfoattachment1data, 
                    //int workinfoattachment1origsize, 
                    //string login_id, 
                    //string customer_company, 
                    //string corporate_id