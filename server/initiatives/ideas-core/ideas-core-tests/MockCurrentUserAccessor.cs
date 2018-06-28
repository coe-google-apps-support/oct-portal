using CoE.Ideas.Shared.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CoE.Ideas.Core.Tests
{
    public class MockCurrentUserAccessor : ICurrentUserAccessor
    {
        private ClaimsPrincipal showWhite;
        protected ClaimsPrincipal SnowWhite
        {
            get
            {
                if (showWhite == null)
                {
                    showWhite = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Snow White"),
                        new Claim(ClaimTypes.Email, "snow.white@edmonton.ca")
                    }, "someAuthTypeName"));
                }
                return showWhite;
            }
        }

        //private ClaimsPrincipal sleepingBeauty;
        //protected ClaimsPrincipal SleepingBeauty
        //{
        //    get
        //    {
        //        if (sleepingBeauty == null)
        //        {
        //            sleepingBeauty = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //            {
        //                new Claim(ClaimTypes.Name, "Sleeping Beauty"),
        //                new Claim(ClaimTypes.Email, "sleepingbeauty@edmonton.ca")
        //            }, "someAuthTypeName"));
        //        }
        //        return sleepingBeauty;
        //    }
        //}

        public ClaimsPrincipal User => SnowWhite;
    }
}
