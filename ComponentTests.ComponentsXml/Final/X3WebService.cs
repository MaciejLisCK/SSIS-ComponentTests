using ComponentTests.ComponentsXml.X3WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.Final
{
    public class X3WebService : CAdxWebServiceXmlCCClient
    {
        private const int MaxReceivedMessageSize = 100000000;
        private const string WebServiceAddressPattern = "http://{0}:{1}/adxwsvc/services/CAdxWebServiceXmlCC";

        public static CAdxCallContext Context
        {
            get
            {
                var context = new CAdxCallContext();
                context.codeLang = "ENG";
                context.codeUser = "ADMWS";
                context.password = "";
                context.poolAlias = "PROTTSG";
                context.requestConfig = "adxwss.trace.on=off&adxwss.trace.size=0&adonix.trace.on=off&adonix.trace.level=3&adonix.trace.size=0";

                return context;
            }
        }

        public X3WebService(string host, int port)
            : base(
                new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedMessageSize }, 
                new EndpointAddress(String.Format(WebServiceAddressPattern, host, port)))
        { }
    }
}
