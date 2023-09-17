using Microsoft.AspNetCore.SignalR;

namespace SignalR_POC.ASP.Hubs;

public class ChatHub : Hub
{
	public async Task SendMessage(string message)
	{

		char[] stringArray = message.ToCharArray();
		Array.Reverse(stringArray);
		string reversedStr = new string(stringArray);

		await Clients.All.SendAsync("ReceiveMessage", reversedStr);
	}
}