using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WitAi;
using WitAi.Models;

namespace DarionMograine
{
     class neuro
    {
        private static string accessToken = "6QAAE5PQGFM3GFCH37QAGCFQRA7JJ4LP";
        public void Brain()
        {
       
        var actions = new WitActions();
        actions["send"] = Send;

        Wit client = new Wit(accessToken: accessToken, actions: actions);

            var response = client.Message("hello dupa");
            string dupa = String.Join("", response.Entities.Values);

          
            dynamic reply = response.Entities.Values;
            


            Console.WriteLine("Yay, got Wit.ai response: " + response.Entities.ToString());
        }

        private static WitContext Send(ConverseRequest request, ConverseResponse response)
        {
            // Do something with the Context
            Console.WriteLine("Yay, got Wit.ai context: " + request.Context);
            return request.Context;
        }
    }
}
