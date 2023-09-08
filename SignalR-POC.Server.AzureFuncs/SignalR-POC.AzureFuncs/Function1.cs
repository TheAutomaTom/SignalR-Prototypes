using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace SignalR_POC.AzureFuncs
{

	public static class Negotiator //SignalRTestHub : ServerlessHub
	{
		[FunctionName("negotiate")]
		public static SignalRConnectionInfo Negotiate(
					[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
					[SignalRConnectionInfo(HubName = "negotiateHub")] SignalRConnectionInfo connectionInfo)
		{
			Console.WriteLine(connectionInfo);
			return connectionInfo;
		}
	}


}
