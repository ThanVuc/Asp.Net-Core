using System.Text;

public static class HtmlHelper
{

	public static string HtmlDocument(string title, string content)
	{
		return $@"
                    <!DOCTYPE html>
                    <html>
                        <head>
                            <meta charset=""UTF-8"">
                            <title>{title}</title>
							<link href=""/manage/bootstrap-5.0.2-dist/css/bootstrap.min.css"" rel=""stylesheet""/>
							<script src=""=/manage/bootstrap-5.0.2-dist/js/bootstrap.min.js""></script>
                        </head>
                        <body>
                            {content}
                        </body>
                    </html>";
	}

	public static string MenuTop(object[] menus, HttpRequest request)
	{

		var menubuilder = new StringBuilder();
		menubuilder.Append("<ul class=\"navbar-nav\">");
		foreach (dynamic menu in menus)
		{
			string _class = "nav-item";
			// Active khi request.PathBase giống url của menu
			if (request.Path == menu.url) _class += " active";
			menubuilder.Append($@"
                                <li class=""{_class}"">
                                    <a class=""nav-link"" href=""{menu.url}"">{menu.label}</a>
                                </li>
                                ");
		}
		menubuilder.Append("</ul>\n");

		string menuhtml = $@"
                    <div class=""container"">
                        <nav class=""navbar navbar-expand-lg navbar-dark bg-dark p-2"">
                            <a class=""navbar-brand"" href=""/"">XTLAB</a>
                            <button class=""navbar-toggler"" type=""button""
                                data-toggle=""collapse"" data-target=""#my-nav-bar""
                                aria-controls=""my-nav-bar"" aria-expanded=""false"" aria-label=""Toggle navigation"">
                                <span class=""navbar-toggler-icon""></span>
                            </button>
                            <div class=""collapse navbar-collapse"" id=""my-nav-bar"">
                                {menubuilder}
                            </div>
                    </nav></div>";

		return menuhtml;
	}

	public static object[] DefaultMenuTopItems()
	{
		return new[] {
			  new {
				  url = "/RequestInfo",
				  label = "Request"
			  },
			  new {
				  url = "/Form",
				  label = "Form"
			  }
			  ,
			  new {
				  url = "/Encoding",
				  label = "Encoding"
			  },
			  new {
				  url = "/Cookies",
				  label = "Cookies"
			  },
			  new {
				  url = "/Json",
				  label = "JSON"
			  }
		  };
	}

	public static string HtmlTrangchu()
	{
		return $@"
          <div class=""container"">
            <div class=""jumbotron"">
                <h1 class=""display-4"">Đây là một trang Web .NET Core</h1>
                <p class=""lead"">Trang Web này xây dựng trên nền tảng  <code>.NET Core</code>,
                chưa sử dụng kỹ thuật MVC - nhằm mục đích học tập.
                Mã nguồn trang này tại <a target=""_blank""
                    href=""https://github.com/xuanthulabnet/learn-cs-netcore/blob/master/ASP_NET_CORE/03.RequestResponse/"">
                    Mã nguồn Ví dụ</a>
                
                </p>


                <hr class=""my-4"">
                <p><code>.NET Core</code> là một hệ thống chạy đa nền tảng (Windows, Linux, macOS)</p>
                <a class=""btn btn-danger btn-lg"" href=""https://xuanthulab.net/lap-trinh-c-co-ban/"" role=""button"">Xem thêm</a>
            </div>
        </div>
         ";

	}
	public static string HtmlTag(this string content, string tag = "p", string _class = null)
	{
		string cls = (_class != null) ? $" class=\"{_class}\"" : null;
		return $"<{tag + cls}>{content}</{tag}>";
	}
	public static string td(this string content, string _class = null)
	{
		return content.HtmlTag("td", _class);
	}
	public static string tr(this string content, string _class = null)
	{
		return content.HtmlTag("tr", _class);
	}
	public static string table(this string content, string _class = null)
	{
		return content.HtmlTag("table", _class);
	}


}