﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Models
{


    // Contract example:
    //{
    //    businessCaseUrl: 'www.google.com/img.png',
    //    assignee: {
    //      name: 'Super BA',
    //      email: 'super.ba@edmonton.ca',
    //      phoneNumber: '555-555-5555',
    //      avatarURL: 'https://www.iconexperience.com/v_collection/icons/?icon=user_generic2_black'
    //    }
    //}

public class Resources
    {
        public User Assignee { get; set; }
    }
}
