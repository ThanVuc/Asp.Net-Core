
using FirstWeb.AspNetCore.Configuration;
using Microsoft.Extensions.Options;

namespace FirstWeb.AspNetCore.IService_MapWhen
{
	public class Laptop : IGetProductName
	{
		public Laptop() => Console.WriteLine("All laptop are created");

		public Laptop(IOptions<ProductConfig> config)
		{
			laptops = config.Value.Laptops;
		}

		string[] laptops;

		public IEnumerable<string> GetNames()
		{
			return laptops;
		}
	}
}
