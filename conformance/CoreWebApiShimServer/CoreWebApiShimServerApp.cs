using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CoreWebApiShimServer
{
	public static class CoreWebApiShimServerApp
	{
		public static void Main()
		{
			const string url = "http://localhost:4117";
			new WebHostBuilder().UseKestrel().UseUrls(url).UseStartup<Startup>().Build().Run();
		}

		private sealed class Startup
		{
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddSingleton<IConformanceApi>(new ConformanceApiService(LoadTests()));
				services.AddMvc();
			}

			[SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Hosting environment not currently used.")]
			public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
			{
			}
		}

		private static IReadOnlyList<ConformanceTestInfo> LoadTests()
		{
			using var testsJsonReader = new StreamReader(typeof(CoreWebApiShimServerApp).Assembly.GetManifestResourceStream("CoreWebApiShimServer.ConformanceTests.json")!);
			return ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd()).Tests!;
		}
	}
}
