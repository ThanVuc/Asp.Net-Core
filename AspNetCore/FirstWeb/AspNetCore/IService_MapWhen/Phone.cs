
using FirstWeb.AspNetCore.Configuration;
using Microsoft.Extensions.Options;

namespace FirstWeb.AspNetCore.IService_MapWhen
{
	public class Phone : IGetProductName
	{
		public Phone() => Console.WriteLine("All Phone are created");

		private List<string> phones;

		public Phone(IOptions<ProductConfig> config)
		{
			phones = config.Value.Phones;
		} 


		public IEnumerable<string> GetNames()
		{
			return phones;
		}
	}
}
