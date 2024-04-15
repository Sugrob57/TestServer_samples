using System;
using Microsoft.Extensions.Configuration;
using TodoApp.MockMappings.Mappings;
using WireMock.Settings;

namespace WireMock.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			RunMockServer("https://localhost:9092");
		}

		public static void RunMockServer(string wireMockUri)
		{
			var stub = WireMockServer.Start(new WireMockServerSettings
			{
				Urls = new[] { wireMockUri },
				StartAdminInterface = true,
			});

			var mock = new MailingAppMappings(stub);
			mock.MockSendEmailRequest();

			Console.WriteLine("WireMock server started");
			Console.WriteLine("Press any key for stop server...");
			Console.ReadKey();
			stub.Stop();
		}
	}
}