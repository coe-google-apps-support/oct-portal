using CoE.Issues.Core.Remedy;
using CoE.Issues.Remedy.Watcher.RemedyServiceReference;
using EnsureThat;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoE.Issues.Remedy.Watcher
{
    internal class RemedyDbReader : IRemedyChangedReceiver, IDisposable
    {
        public RemedyDbReader(Serilog.ILogger logger,
            string connectionString)
        {
            EnsureArg.IsNotNull(logger);
            EnsureArg.IsNotNullOrWhiteSpace(connectionString);
            _logger = logger;
            _oracleConnection = new OracleConnection(connectionString);
        }

        private readonly Serilog.ILogger _logger;
        private OracleConnection _oracleConnection;

        public void ReceiveChanges(DateTime fromDateUtc,
            Func<Incident, CancellationToken, Task> incidentUpdatedHandler = null,
            RemedyReaderOptions options = null)
        {
            DateTime toDateUtc = DateTime.UtcNow;
            PollRemedyIncidents(incidentUpdatedHandler, fromDateUtc, toDateUtc);

            //Poll work orderse here...
        }

        private void PollRemedyIncidents(Func<Incident, CancellationToken, Task> incidentUpdatedHandler,
            DateTime fromDateUtc, DateTime toDateUtc)
        {
            if (incidentUpdatedHandler != null)
            {
                var cmd = _oracleConnection.CreateCommand();
                cmd.CommandText = INCIDENT_SQL;
                var fromDateParam = cmd.CreateParameter();
                fromDateParam.Value = fromDateUtc;
                cmd.Parameters.Add(fromDateParam);
                var toDateParam = cmd.CreateParameter();
                toDateParam.Value = toDateUtc;
                cmd.Parameters.Add(toDateParam);
                try
                {
                    Stopwatch watch = new Stopwatch();
                    int rowCount = 0;
                    CancellationToken cancellationToken = default(CancellationToken);
                    watch.Start();
                    _oracleConnection.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Incident incident = CreateEntityFromDbRow<Incident>(reader);
                        ThreadPool.QueueUserWorkItem(o =>
                        {
                            Incident i = (Incident)o;
                            if (i.LAST_MODIFIED_DATE == i.ORIGINAL_LAST_MODIFIED_DATE)
                                incidentUpdatedHandler(i, cancellationToken).Wait(cancellationToken);
                        }, incident);
                        rowCount++;
                    }
                    _logger.Information("Read {RowCount} rows in {ElapsedMilliseconds}ms", rowCount, watch.ElapsedMilliseconds);
                }
                catch (Exception err)
                {
                    _logger.Error(err, "Error reading from Remedy database: {ErrorMessage}", err.Message);
                }
                finally
                {
                    _oracleConnection.Close();
                }
            }
        }

        private IDictionary<PropertyInfo, int> GetIncidentPropertymap(DbDataReader reader)
        {
            throw new NotImplementedException();
        }

        private T CreateEntityFromDbRow<T>(DbDataReader reader) where T : class
        {
            var propertyMap = GetIncidentPropertymap(reader);
            var returnValue = Activator.CreateInstance<T>();
            foreach (var prop in propertyMap.Keys)
            {
                prop.SetValue(returnValue, reader[propertyMap[prop]]);
            }
            return returnValue;
        }


        private void PollRemedyWorkItems(Func<WorkOrderCreatedEventArgs, CancellationToken, Task> workOrderCreatedHandler)
        {
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    try { _oracleConnection.Dispose(); } catch (Exception) { /* eat exceptions */ }
                    _oracleConnection = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RemedyDbReader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


        #region SQL
        private const string INCIDENT_SQL = @"
SELECT ENTRY_ID 
    ,SEND_PAGE_ 
    ,SUBMITTER
    ,SUBMIT_DATE    
    ,ASSIGNEE_LOGIN_ID              
    ,LAST_MODIFIED_BY               
    ,LAST_MODIFIED_DATE             
    ,STATUS                         
    ,SHORT_DESCRIPTION              
    ,SUBMITTED_PRODCAT_TIER1        
    ,SUBMITTED_PRODCAT_TIER2        
    ,SUBMITTED_PRODCAT_TIER3        
    ,SUBMITTED_PRODCAT_PRODNAME     
    ,VENDOR_NAME                    
    ,RESOLUTION_CATEGORY            
    ,OWNER_SUPPORT_COMPANY          
    ,OWNER_GROUP_ID                 
    ,LOOKUPKEYWORD                  
    ,OWNER_GROUP                    
    ,IMPACT_OR_ROOT                 
    ,ASSIGNEE_GROUPS                
    ,HPD_CI_FORMNAME                
    ,STATUS_REASON2                 
    ,ASSIGNED_GROUP_SHIFT_NAME      
    ,SHIFTS_FLAG                    
    ,INSTANCEID                     
    ,ASSIGNED_GROUP_SHIFT_ID        
    ,CURRENTSTAGENUMBER             
    ,OWNER_SUPPORT_ORGANIZATION     
    ,DO_NOT_ARCHIVE                 
    ,ORIGINAL_LAST_MODIFIED_DATE    
    ,RESOLUTION                     
    ,ADDITIONAL_LOCATION_DETAILS    
    ,STATUS_REASON                  
    ,DETAILED_DECRIPTION            
    ,STATUS_INCIDENT                
    ,CURRENTSTAGE                   
    ,COST_CENTER                    
    ,PRIORITY_WEIGHT                
    ,URGENCY                        
    ,IMPACT                         
    ,INCIDENT_NUMBER                
    ,PRIORITY                       
    ,ASSIGNEE                       
    ,ASSIGNED_GROUP                 
    ,REPORTED_SOURCE                
    ,ASSIGNED_SUPPORT_COMPANY       
    ,Z1D_SERVICEHEALTH              
    ,VENDOR_PHONE                   
    ,VIP                            
    ,CONTACT_SENSITIVITY            
    ,Z1D_TOTALBREACHEDINCIDENTSCOUN 
    ,LOCAL_PHONE                    
    ,EXTENSION                      
    ,COUNTRY_CODE                   
    ,AREA_CODE                      
    ,LAST_NAME                      
    ,FIRST_NAME                     
    ,FULL_NAME                      
    ,COMPONENT_ID                   
    ,CONTACT_CLIENT_TYPE            
    ,MIDDLE_INITIAL                 
    ,ORGANIZATION                   
    ,TEMPLATEID                     
    ,FLAG_CREATE_REQUEST            
    ,ASSIGNED_SUPPORT_ORGANIZATION  
    ,COUNTRY                        
    ,STATE_PROVINCE                 
    ,DESCRIPTION                    
    ,COMPANY                        
    ,CITY                           
    ,PHONE_NUMBER                   
    ,PRODUCT_MODEL_VERSION          
    ,PRODUCT_NAME                   
    ,MANUFACTURER                   
    ,CATEGORIZATION_TIER_1          
    ,INTERNET_E_MAIL                
    ,CORPORATE_ID                   
    ,GEONET                         
    ,DESK_LOCATION                  
    ,ZIP_POSTAL_CODE                
    ,MAIL_STATION                   
    ,STREET                         
    ,KMSGUID                        
    ,CONTACT_COMPANY                
    ,PERSON_ID                      
    ,SITE_ID                        
    ,ASSIGNED_GROUP_ID              
    ,CATEGORIZATION_TIER_2          
    ,CATEGORIZATION_TIER_3          
    ,HR_ID                          
    ,REGION                         
    ,PRODUCT_CATEGORIZATION_TIER_1  
    ,Z1D_TOTALCRITICALINCIDENTSCOUN 
    ,SITE_GROUP                     
    ,STATUS_PPL                     
    ,DEPARTMENT                     
    ,PRODUCT_CATEGORIZATION_TIER_3  
    ,PRODUCT_CATEGORIZATION_TIER_2  
    ,CI_TAG_NUMBER                  
    ,SERVICE_TYPE                   
    ,SRID                           
    ,SATISFACTION_RATING            
    ,ESCHAT_SET_AUTO_ASSIGN         
    ,Z1D_CI_FORMNAME                
    ,PREVIOUSSTATUS                 
    ,Z1D_VISPROCESSFLOWVIEW         
    ,SERVICECI                      
    ,LOGIN_ID                       
    ,CHAT_SESSION_ID                
    ,DIRECT_CONTACT_SITE_ID         
    ,DIRECT_CONTACT_MAIL_STATION    
    ,DIRECT_CONTACT_LOCATION_DETAIL 
    ,DIRECT_CONTACT_LOCAL_NUMBER    
    ,DIRECT_CONTACT_EXTENSION       
    ,DIRECT_CONTACT_COUNTRY_CODE    
    ,DIRECT_CONTACT_AREA_CODE       
    ,DIRECT_CONTACT_COUNTRY         
    ,DIRECT_CONTACT_STATE_PROVINCE  
    ,DIRECT_CONTACT_STREET          
    ,DIRECT_CONTACT_TIME_ZONE       
    ,DIRECT_CONTACT_DESK_LOCATION   
    ,DIRECT_CONTACT_CITY            
    ,SERVICECI_CLASS                
    ,DIRECT_CONTACT_ZIP_POSTAL_CODE 
    ,SLA_BREACH_EXCEPTION           
    ,CREATED_FROM_TEMPLATE          
    ,CLOSURE_SOURCE                 
    ,CLOSURE_MANUFACTURER           
    ,Z2AF_WORK_LOG03001             
    ,SLA_BREACH_REASON              
    ,RESOLUTION_CATEGORY_TIER_3     
    ,CLOSURE_PRODUCT_CATEGORY_TIER1 
    ,RESOLUTION_METHOD              
    ,RESOLUTION_CATEGORY_TIER_2     
    ,CLOSURE_PRODUCT_NAME           
    ,Z2AF_WORK_LOG01001             
    ,KICKBACK_COUNT                 
    ,CLOSURE_PRODUCT_MODEL_VERSION  
    ,Z2AF_WORK_LOG02001             
    ,CLOSURE_PRODUCT_CATEGORY_TIER2 
    ,HPD_CI                         
    ,CLOSURE_PRODUCT_CATEGORY_TIER3 
    ,LAST_NAME2                     
    ,SUPPORT_GROUP_ROLE             
    ,ENABLE_ASSIGNMENT_ENGINE       
    ,ATTACHMENT                     
    ,CELL_NAME                      
    ,DIRECT_CONTACT_ORGANIZATION    
    ,DIRECT_CONTACT_DEPARTMENT      
    ,DIRECT_CONTACT_MIDDLE_INITIAL  
    ,DIRECT_CONTACT_PHONE_NUMBER    
    ,DIRECT_CONTACT_SITE            
    ,DIRECT_CONTACT_PERSON_ID       
    ,DIRECT_CONTACT_REGION          
    ,DIRECT_CONTACT_SITE_GROUP      
    ,TEMPLATEGUID                   
    ,DIRECT_CONTACT_LAST_NAME       
    ,DIRECT_CONTACT_FIRST_NAME      
    ,DIRECT_CONTACT_COMPANY         
    ,FIRST_NAME2                    
    ,GROUP_TRANSFERS                
    ,TOTAL_TRANSFERS                
    ,INDIVIDUAL_TRANSFERS           
    ,VENDOR_LAST_NAME               
    ,VENDOR_FIRST_NAME              
    ,VENDOR_EMAIL                   
    ,VENDOR_RESPONDED_ON            
    ,VENDOR_PERSON_ID               
    ,VENDOR_RESOLVED_DATE           
    ,VENDOR_GROUP2                  
    ,LAST_KICKBACK_DATE             
    ,VENDOR_LOGIN_ID                
    ,Z2AF_WORK_LOG01002             
    ,VENDOR_ASSIGNMENT_STATUS       
    ,INCAUTOCLOSERESOLVED_SEC       
    ,CREATE_IMPACTED_AREA_FROM_CUST 
    ,OPTIONFORCLOSINGINCIDENT       
    ,Z1D_VENDORACCESS               
    ,REQUIRED_RESOLUTION_DATETIME   
    ,SLA_RES_BUSINESS_HOUR_SECONDS  
    ,CLIENTLOCALE                   
    ,Z2AF_WORK_LOG02002             
    ,Z2AF_WORK_LOG03002             
    ,VENDOR_GROUP_ID                
    ,ZD_NEXTDUEDATE_TIME            
    ,VENDOR_ORGANIZATION            
    ,VENDOR_GROUP                   
    ,ASSIGNMENT_METHOD              
    ,BORPHANEDROOT                  
    ,Z1D_VISTARGETFORM              
    ,OUTBOUND                       
    ,INBOUND                        
    ,HPD_TEMPLATE_ID                
    ,DR                             
    ,EH                             
    ,SERVICECI_RECONID              
    ,BIIARS_03                      
    ,ESCALATED_                     
    ,RETURN_CODE                    
    ,SRINSTANCEID                   
    ,SLA_RESPONDED                  
    ,OLA_HOLD                       
    ,SLMEVENTLOOKUPTBLKEYWORD       
    ,RESOLUTION_START_DATE          
    ,BIIARS_02                      
    ,ACKNOWLEDGMENT_START_DATE      
    ,UNKNOWNUSER                    
    ,REASON_DESCRIPTION             
    ,POLICY_TYPE                    
    ,WEB_INCIDENT_ID                
    ,ESTIMATED_RESOLUTION_DATE      
    ,ROOT_INCIDENT_ID_LIST          
    ,BIIARS_01                      
    ,REASON_CODE                    
    ,MC_UEID                        
    ,TOTAL_FIELDS_COUNT             
    ,LAST__ASSIGNED_DATE            
    ,CREATED_FROM_FLAG              
    ,ABYDOS_NOTIFY_TEXT             
    ,ABYDOS_NOTIFY_RECIPIENT        
    ,ABYDOS_PROCESS_VERSION         
    ,ABYDOS_PROCESS_STATUS          
    ,ABYDOS_USE_WIZARD_             
    ,ABYDOS_TASKS_GENERATED         
    ,ACTIVE_TASKS                   
    ,SLM_PRIORITY                   
    ,DIRECT_CONTACT_INTERNET_E_MAIL 
    ,TICKETTYPE                     
    ,USE_CASE                       
    ,SLMLOOKUPTBLKEYWORD            
    ,BROADCASTED_FLAG               
    ,CI                             
    ,ASSIGNEE_SELECT_FORM           
    ,ABYDOS_PROCESS_NAME            
    ,ABYDOS_TEMPLATE_ID             
    ,ABYDOS_SLA_NAME                
    ,ABYDOS_AUDITFLAG               
    ,COMMAND                        
    ,Z1D_VISFORMVIEW                
    ,DIRECT_CONTACT_CORPORATE_ID    
    ,Z1D_TEMPLATE_NAME              
    ,ROOT_COMPONENT_ID_LIST         
    ,VENDOR_ASSIGNEE_GROUPS         
    ,USER_ID_PERMISSIONS            
    ,GLOBAL_OR_CUSTOM_MAPPING       
    ,SERVICE                        
    ,POLICY_NAME                    
    ,CREATED_BY                     
    ,ROOTREQUESTNAME                
    ,PATCH_LAST_BUILD_ID            
    ,ASSIGN_TO_VENDOR               
    ,CATEGORY                       
    ,HPD_CI_RECONID                 
    ,REPRODUCEABLE_FLAG             
    ,EFFORTDURATIONHOUR             
    ,ORIGINAL_LAST_MODIFIED_BY      
    ,SRMSAOIGUID                    
    ,SLM_STATUS                     
    ,INFRASTRUCTURE_CHG_INITIATED   
    ,SITE                           
    ,INFRASTRUCTUREEVENTTYPE        
    ,SRMS_REGISTRY_INSTANCE_ID      
    ,VENDOR_CONTACT                 
    ,Z1G_DEFAULTVUI                 
    ,ORIGINAL_INCIDENT_NUMBER       
    ,REPORTED_TO_VENDOR             
    ,INCIDENT_ASSOCIATION_TYPE      
    ,CUSTOMER_LOGIN_ID              
    ,VENDOR_TICKET_NUMBER           
    ,EFFORT_TIME_SPENT_MINUTES      
    ,SHOW_FOR_PROCESS               
    ,DIRECT_CONTACT_LOGIN_ID        
    ,TOTAL_TIME_SPENT               
    ,OWNER                          
    ,ASSOCIATION_DESCRIPTION        
    ,OWNER_LOGIN_ID                 
    ,GENERIC_CATEGORIZATION_TIER_3  
    ,STAGECONDITION                 
    ,GENERIC_CATEGORIZATION_TIER_1  
    ,GENERIC_CATEGORIZATION_TIER_2  
    ,ASSIGNEE_ID                    
    ,TIME_ZONE                      
    ,BIIARS_05                      
    ,ONWER_GROUP_USES_SLA           
    ,SEV1SERVICE                    
    ,PREVIOUS_ORGANIZATION          
    ,ASSIGNED_GROUP_USES_OLA        
    ,LAST_ACKNOWLEDGED_DATE         
    ,LAST_RESOLVED_DATE             
    ,REPORTED_DATE                  
    ,RESPONDED_DATE                 
    ,RE_OPENED_DATE                 
    ,SLA_HOLD                       
    ,CLOSED_DATE                    
    ,LAST_SLA_HOLD_DATE             
    ,NEXT_TARGET_DATE               
    ,TOTAL_ESCALATION_LEVEL         
    ,TOTAL_OLA_RESOLUTION_ESC_LEVEL 
    ,TOTAL_OLA_ACKNOWLEDGEESC_LEVEL 
    ,INITAL_ASSIGNED_DATE           
    ,SLM_MESSAGE_BODY__C            
    ,SLM_SUBJECT                    
    ,SRATTACHMENT                   
    ,COE_SUBMITTERDEFAULTGROUP      
    ,DATATAGS                       
    ,LAST_DATE_DURATION_CALCULATED  
    ,BIIARS_04                      
FROM ARADMIN.HPD_HELP_DESK
WHERE LAST_MODIFIED_DATE > :fromDate AND LAST_MODIFIED_DATE <= :toDate";
        #endregion
    }
}
