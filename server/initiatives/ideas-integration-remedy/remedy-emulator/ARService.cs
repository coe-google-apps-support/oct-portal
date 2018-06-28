using CoE.Ideas.Remedy.RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Remedy.Emulator
{
    public class ARService : New_Port_0PortType
    {
        public New_Create_Operation_0Response New_Create_Operation_0(New_Create_Operation_0Request request)
        {
            return new New_Create_Operation_0Response(Guid.NewGuid().ToString());
        }

        public Task<New_Create_Operation_0Response> New_Create_Operation_0Async(New_Create_Operation_0Request request)
        {
            return Task.FromResult(new New_Create_Operation_0Response(Guid.NewGuid().ToString()));
        }
    }
}
