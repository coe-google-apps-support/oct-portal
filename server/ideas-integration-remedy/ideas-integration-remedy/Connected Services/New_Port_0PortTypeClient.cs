
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Remedy.RemedyServiceReference
{
    public partial class New_Port_0PortTypeClient
    {
        public System.Threading.Tasks.Task<CoE.Ideas.Remedy.RemedyServiceReference.New_Create_Operation_0Response> New_Create_Operation_0Async(
            CoE.Ideas.Remedy.RemedyServiceReference.AuthenticationInfo AuthenticationInfo, 
            string Customer_Company,
            string Customer_Login_ID,
            string Customer_First_Name,
            string Customer_Last_Name,
            string z1D_Action, 
            string Summary, 
            string Description, 
            System.Nullable<Requested_ForType> Requested_For, 
            string Location_Company,
            string Work_Order_Template_Used, 
            string TemplateName)
        {
            CoE.Ideas.Remedy.RemedyServiceReference.New_Create_Operation_0Request inValue = new CoE.Ideas.Remedy.RemedyServiceReference.New_Create_Operation_0Request();
            inValue.AuthenticationInfo = AuthenticationInfo;
            inValue.Customer_Company = Customer_Company;
            inValue.Customer_Login_ID = Customer_Login_ID;
            inValue.Customer_First_Name = Customer_First_Name;
            inValue.Customer_Last_Name = Customer_Last_Name;
            inValue.z1D_Action = z1D_Action;
            inValue.Summary = Summary;
            inValue.Description = Description;
            inValue.Requested_For = Requested_For;
            inValue.Location_Company = Location_Company;
            inValue.Work_Order_Template_Used = Work_Order_Template_Used;
            inValue.TemplateName = TemplateName;
            return ((CoE.Ideas.Remedy.RemedyServiceReference.New_Port_0PortType)(this)).New_Create_Operation_0Async(inValue);
        }

    }
}
