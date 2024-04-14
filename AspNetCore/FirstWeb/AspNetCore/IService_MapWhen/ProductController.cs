using Newtonsoft.Json;
using System.Text;

namespace FirstWeb.AspNetCore.IService_MapWhen
{
	public class ProductController
	{
		IGetProductName lsLaptop;
		IGetProductName lsPhone;

		public ProductController(IGetProductName laptops, Phone phones)
		{
			this.lsLaptop = laptops;
			this.lsPhone = phones;
		}

		public string List(HttpContext context)
		{

			var sb = new StringBuilder();
			sb.Append(CountAccess(context).HtmlTag("p","text-danger"));
			string lsPhoneHTML = string.Join("", lsPhone.GetNames().Select(name => name.HtmlTag("li"))).HtmlTag("ul");
			string lsLaptopHTML = string.Join("", lsLaptop.GetNames().Select(name => name.HtmlTag("li"))).HtmlTag("ul");
			sb.Append("Danh sách điện thoại".HtmlTag("h2"));
			sb.Append(lsPhoneHTML);

			sb.Append("Danh sách Laptop".HtmlTag("h2"));
			sb.Append(lsLaptopHTML);

			string html = HtmlHelper.HtmlDocument("DS Sản phẩm", sb.ToString().HtmlTag("div", "container"));
			return html;
		}

		public string CountAccess(HttpContext context)
		{
			ISession session = context.Session;
			string key = "countInfo";

			var initAccess = new
			{
				nums = 0,
				time = DateTime.Now.ToString()
			};

			dynamic currentAccess;

			string? json = session.GetString(key);

			if (json == null)
			{
				// create session obj
				currentAccess = initAccess;
			} else
			{
				// take session obj
				currentAccess = JsonConvert.DeserializeObject(json,initAccess.GetType());
			}

			//logic handle session obj

			var saveAccess = new
			{
				nums = currentAccess.nums +1,
				time = DateTime.Now.ToString()
			};

			//save session obj

			string jsonSave = JsonConvert.SerializeObject(saveAccess);
			session.SetString(key,jsonSave);

			//return info
			return $"Session: {key}, Access Nums: {saveAccess.nums}, DateTime: {saveAccess.time}";
		}


	}
}
