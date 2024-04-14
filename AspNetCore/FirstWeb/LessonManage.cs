using FirstWeb.AspNetCore.Configuration;
using FirstWeb.AspNetCore.IService_MapWhen;
using FirstWeb.AspNetCore.map_request_response;
using FirstWeb.AspNetCore.SendMail;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace FirstWeb
{
	public class LessonManage
	{
		//MiddleWare
		public static void ss59(WebApplicationBuilder builder)
		{
			builder.WebHost.ConfigureKestrel(options =>
			{
				options.Limits.MaxRequestBodySize = 104857600;
			});

			var app = builder.Build();

			app.UseStaticFiles();


			app.UseRouting();


			app.MapGet("/requestinfo", async context =>
			{
				string menu = HtmlHelper.MenuTop(HtmlHelper.DefaultMenuTopItems(), context.Request);
				string requestInfo = RequestProccess.Gen_RequestInfo(context.Request);
				string content = menu + requestInfo.HtmlTag("div", "container");
				string html = HtmlHelper.HtmlDocument("RequestInfo", content);
				await context.Response.WriteAsync(html);
			});

			app.MapMethods("/form", new string[] { "POST", "GET" }, async context =>
			{
				var menu = HtmlHelper.MenuTop(HtmlHelper.DefaultMenuTopItems(), context.Request);
				string content = await RequestProccess.GenAndHandle_Form(context.Request);
				Console.WriteLine(content);
				string html = HtmlHelper.HtmlDocument("Form", menu + content);
				await context.Response.WriteAsync(html);
			});

			app.MapGet(@"/cookies", async context =>
			{
				string menu = HtmlHelper.MenuTop(HtmlHelper.DefaultMenuTopItems(), context.Request);
				var action = context.GetRouteValue("action") ?? "read";
				string huongdan = "<div class=\"list-group\">\r\n      <a class=\"list-group-item\" href=\"/Cookies/read\">Đọc Cookies</a>\r\n      <a class=\"list-group-item\" href=\"/Cookies/write\">Ghi Cookies</a>\r\n  </div>";
				string html = HtmlHelper.HtmlDocument("Cookies: " + action.ToString(), (menu + huongdan.HtmlTag("div", "container")));
				await context.Response.WriteAsync(html);
			});

			app.MapGet(@"/cookies/{action}", async context =>
			{
				string menu = HtmlHelper.MenuTop(HtmlHelper.DefaultMenuTopItems(), context.Request);
				var action = context.GetRouteValue("action") ?? "read";
				string cookies = RequestProccess.Gen_Cookies(context.Request, context.Response).HtmlTag("div", "container");
				string html = HtmlHelper.HtmlDocument("Cookies: " + action.ToString(), (menu + cookies));
				await context.Response.WriteAsync(html);
			});

			app.MapGet("/encoding", async context =>
			{
				string menu = HtmlHelper.MenuTop(HtmlHelper.DefaultMenuTopItems(), context.Request);
				string htmlec = RequestProccess.Encoding(context.Request).HtmlTag("div", "container");
				string html = HtmlHelper.HtmlDocument("Encoding", (menu + htmlec));
				await context.Response.WriteAsync(html);
			});

			app.Map("/json", app =>
			{
				app.Run(async context =>
				{
					var json = RequestProccess.Gen_Json();
					context.Response.ContentType = "application/json";

					await context.Response.WriteAsync(json);
				});
			});

			app.MapGet("/", async context =>
			{
				string menu = HtmlHelper.MenuTop(HtmlHelper.DefaultMenuTopItems(), context.Request);
				string content = HtmlHelper.HtmlTrangchu();

				string html = HtmlHelper.HtmlDocument("Trang chủ", menu + content);
				await context.Response.WriteAsync(html);
			});

			app.Run();

		}

		//Map - Request - Response
		public static void ss60(WebApplicationBuilder builder)
		{
			var services = builder.Services;
			services.AddSingleton<IGetProductName, Laptop>();
			services.AddSingleton<Phone, Phone>();
			services.AddSingleton<ProductController, ProductController>();

			var app = builder.Build();

			app.UseStaticFiles();

			app.UseRouting();

			app.Map("/product", async context =>
				{
					var menu = HtmlHelper.MenuTop(new[]
					{
						new
						{
							url = "/product",
							lable = "Product"
						},
						new
						{
							url = "/allservices",
							lable="All Services"
						}
					}, context.Request);
					var productController = context.RequestServices.GetService<ProductController>();
					var productList = productController.List(context);

					var html = HtmlHelper.HtmlDocument("Product", menu + productList);

					context.Response.StatusCode = 200;
					await context.Response.WriteAsync(html);

				});

			app.Map("/allservices", async (context) =>
			{
				StringBuilder stringBuilder = new StringBuilder();
				var menu = HtmlHelper.MenuTop(new[]
					{
						new
						{
							url = "/product",
							lable = "Product"
						},
						new
						{
							url = "/allservices",
							lable="All Services"
						}
					}, context.Request);

				stringBuilder.Append("<tr><th>Name</th><th>Life Time</th><th>Full Name</th></tr>");

				foreach (var service in services)
				{
					string tr = service.ServiceType.Name.ToString().HtmlTag("td") +
						service.Lifetime.ToString().HtmlTag("td") +
						service.ServiceType.FullName.ToString().HtmlTag("td");
					stringBuilder.Append(tr.HtmlTag("tr"));
				}



				var html = HtmlHelper.HtmlDocument("AllServices", menu + stringBuilder.ToString().HtmlTag("table", "table table-bordered table-sm"));

				await context.Response.WriteAsync(html);
			});

			app.Map("/", async context =>
			{
				var menu = HtmlHelper.MenuTop(new[] {
			  new {
				  url = "/product",
				  label = "Product"
			  },
			  new {
				  url = "/allservices",
				  label = "AllServices"
			  }}, context.Request);
				var html = HtmlHelper.HtmlDocument("Homes", menu);
				context.Response.WriteAsync(html);
			});

			app.Run();
		}

		//Session
		public static void ss61(WebApplicationBuilder builder)
		{
			var services = builder.Services;
			services.AddDistributedSqlServerCache(opt =>
			{
				opt.ConnectionString = "server = localhost;database = webdb;UID = sa;PWD = 88888888;" +
				"TrustServerCertificate = true";
				opt.SchemaName = "dbo";
				opt.TableName = "session";
			});
			services.AddSession(opt =>
			{
				opt.Cookie.Name = "info";
				opt.IOTimeout = new TimeSpan(0, 60, 0);
			});
			services.AddSingleton<IGetProductName,Laptop>();
			services.AddSingleton<Phone, Phone>();
			services.AddSingleton<ProductController, ProductController>();

			var app = builder.Build();

			app.UseStaticFiles();

			app.UseSession();

			app.UseRouting();

			app.Map("/product", async context =>
			{
				var menu = HtmlHelper.MenuTop(new[]
				{
					new
					{
						url = "/product",
						label = "Product"
					}
				}, context.Request);
				var productController = context.RequestServices.GetService<ProductController>();
				var productList = productController.List(context);

				var html = HtmlHelper.HtmlDocument("Product", menu + productList);

				context.Response.StatusCode = 200;
				await context.Response.WriteAsync(html);

			});

			app.Map("/", async context =>
			{
				var menu = HtmlHelper.MenuTop(new[]
				{
					new {
						url = "/product",
						label = "Product"
					}
				}, context.Request);
				var productController = context.RequestServices.GetService<ProductController>();
				var content = productController.List(context);


				var html = HtmlHelper.HtmlDocument("Product", menu + content);

				context.Response.StatusCode = 200;
				await context.Response.WriteAsync(html);
			});

			app.Run();
		}

		//Configuration
		public static void ss62(WebApplicationBuilder builder)
		{
			var services = builder.Services;
			services.AddSingleton<IGetProductName, Laptop>();
			services.AddSingleton<Phone, Phone>();
			services.AddSingleton<ProductController, ProductController>();
			services.AddDistributedMemoryCache();
			services.AddSession(config =>
			{
				config.Cookie.Name = "sinh";
				config.IOTimeout = new TimeSpan(0,50,0);
			});

			services.AddOptions();
			var configuration = builder.Configuration;

			var productDefault = configuration.GetSection("Product");

			services.Configure<ProductConfig>(productDefault);

			

			var app = builder.Build();

			app.UseStaticFiles();
			app.UseSession();
			app.UseRouting();


			app.Map("/product/{*action}", async (HttpContext context) =>
			{
				var productController = context.RequestServices.GetService<ProductController>();
				var content = productController.List(context);

				var menu = HtmlHelper.MenuTop(new[]
				{
					new
					{
						url = "/product",
						label = "Products"
					}
				}, context.Request);

				var s = context.GetRouteValue("action")??"read";
				Console.WriteLine("Route Value: " + s.ToString());

				var html = HtmlHelper.HtmlDocument("Products", menu + content);

				await context.Response.WriteAsync(html);

			});

			app.Map("/", async context =>
			{
				await context.Response.WriteAsync("Homes");
			});

			app.Run();

		}

		//Send Mail - MailKit
		public static void ss63(WebApplicationBuilder builder)
		{
			var services = builder.Services;

			services.AddTransient<ISendMail, SendMail>();
			
			services.AddOptions();                                         // Kích hoạt Options
			var mailsettings = builder.Configuration.GetSection("MailSettings");  // đọc config
			services.Configure<MailSettings>(mailsettings);


			var app = builder.Build();

			app.UseStaticFiles();
			app.UseRouting();
			app.MapGet("/", async context => {
				await context.Response.WriteAsync("Hello World!");
			});

			app.Map("/sendmail", async context =>
			{
				string body = "<p><strong>Xin chào Sinh</strong></p>";
				var mail = context.RequestServices.GetService<ISendMail>();

				string annouce = await mail.SendMailAsync("sinhnguyen417@gmail.com","Test1",body);
				await context.Response.WriteAsync(annouce);
			});

			app.Run();

		}
	}
}
