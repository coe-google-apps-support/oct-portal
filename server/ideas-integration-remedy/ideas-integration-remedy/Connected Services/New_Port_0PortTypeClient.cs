﻿//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace RemedyServiceReference
//{
//    public partial class New_Port_0PortTypeClient
//    {
//        public System.Threading.Tasks.Task<RemedyServiceReference.New_Create_Operation_0Response> New_Create_Operation_0Async(
//            RemedyServiceReference.AuthenticationInfo AuthenticationInfo,
//            string Customer_Login_ID,
//            string Summary,
//            string Description,
//           // Requested_ForType? Requested_For,
//            string Location_Company,
//            string Customer_Company,
//            string TemplateId,
//            string Customer_First_Name,
//            string Customer_Last_Name,
//            string Categorization_Tier_1,
//            string Categorization_Tier_2)
//        {
//            New_Create_Operation_0Request inValue = new New_Create_Operation_0Request();
//            inValue.AuthenticationInfo = AuthenticationInfo;

//            // hard code some values

//            inValue.Location_Company = Location_Company;
//            inValue.Customer_Company = Customer_Company;
//            inValue.Customer_Login_ID = Customer_Login_ID;
//            inValue.z1D_Action = "Create";
//            inValue.Summary = Summary;
//            inValue.Short_Description = Description;
//            //inValue.Requested_For = Requested_For;
//            inValue.Customer_Company = Customer_Company;
//            inValue.Customer_First_Name = Customer_First_Name;
//            inValue.Customer_Last_Name = Customer_Last_Name;
//            //inValue.WorkOrder = WorkOrder;
//            inValue.Categorization_Tier_1 = Categorization_Tier_1;
//            inValue.Categorization_Tier_2 = Categorization_Tier_2;

//            inValue.TemplateID = TemplateId;

//            return ((RemedyServiceReference.New_Port_0PortType)(this)).New_Create_Operation_0Async(inValue);
//        }

//    }

//    public partial class New_Create_Operation_0Request
//    {
//        //[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "urn:Customer_First_Name", Order = 9)]
//        //public string Customer_First_Name;

//        //[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "urn:Customer_Last_Name", Order = 10)]
//        //public string Customer_Last_Name;

//        //[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "urn:WorkOrder", Order = 11)]
//        //public string WorkOrder;

//        //[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "urn:Categorization_Tier_1", Order = 11)]
//        //public string Categorization_Tier_1;

//        //[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "urn:Categorization_Tier_2", Order = 12)]
//        //public string Categorization_Tier_2;

//        //[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "urn:TemplateID", Order = 13)]
//        //public string TemplateID;
//    }

//}
