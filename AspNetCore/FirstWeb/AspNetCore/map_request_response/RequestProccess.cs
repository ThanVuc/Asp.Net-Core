using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;

namespace FirstWeb.AspNetCore.map_request_response
{
    public class RequestProccess
    {
        public static string Gen_RequestInfo(HttpRequest request)
        {
            var sb = new StringBuilder();

            // Lấy http scheme (http|https)
            var scheme = request.Scheme;
            sb.Append(("scheme".td() + scheme.td()).tr());

            // HOST Header
            var host = request.Host.HasValue ? request.Host.Value : "no host";
            sb.Append(("host".td() + host.td()).tr());


            // Lấy pathbase (URL Path - cho Map)
            var pathbase = request.PathBase.ToString();
            sb.Append(("pathbase".td() + pathbase.td()).tr());

            // Lấy Path (URL Path)
            var path = request.Path.ToString();
            sb.Append(("path".td() + path.td()).tr());

            // Lấy chuỗi query của URL
            var QueryString = request.QueryString.HasValue ? request.QueryString.Value : "no query string";
            sb.Append(("QueryString".td() + QueryString.td()).tr());

            // Lấy phương thức
            var method = request.Method;
            sb.Append(("Method".td() + method.td()).tr());

            // Lấy giao thức
            var Protocol = request.Protocol;
            sb.Append(("Protocol".td() + Protocol.td()).tr());

            // Lấy ContentType
            var ContentType = request.ContentType;
            sb.Append(("ContentType".td() + ContentType.td()).tr());

            // Lấy danh sách các Header và giá trị  của nó, dùng Linq để lấy
            // Header gửi đến lưu trong thuộc tính Header  kiểu Dictionary
            var listheaderString = request.Headers.Select((header) => $"{header.Key}: {header.Value}".HtmlTag("li"));
            var headerhmtl = string.Join("", listheaderString).HtmlTag("ul"); // nối danh sách thành 1
            sb.Append(("Header".td() + headerhmtl.td()).tr());

            // Lấy danh sách các Header và giá trị  của nó, dùng Linq để lấy
            var listcokie = request.Cookies.Select((header) => $"{header.Key}: {header.Value}".HtmlTag("li"));
            var cockiesHtml = string.Join("", listcokie).HtmlTag("ul");
            sb.Append(("Cookies".td() + cockiesHtml.td()).tr());


            // Lấy tên và giá trí query
            var listquery = request.Query.Select((header) => $"{header.Key}: {header.Value}".HtmlTag("li"));
            var queryhtml = string.Join("", listquery).HtmlTag("ul");
            sb.Append(("Các Query".td() + queryhtml.td()).tr());

            //Kiểm tra thử query tên abc có không
            StringValues abc;
            bool existabc = request.Query.TryGetValue("abc", out abc);
            string queryVal = existabc ? abc.FirstOrDefault() : "không có giá trị";
            sb.Append(("abc query".td() + queryVal.ToString().td()).tr());

            string info = "Thông tin Request".HtmlTag("h2") + sb.ToString().HtmlTag("table", "table table-sm table-bordered");
            return info;
        }

        public static string Gen_Json()
        {
            var json = new
            {
                name = "sinh",
                age = "18",
                gender = 1
            };

            return JsonConvert.SerializeObject(json);
        }

        public static string Encoding(HttpRequest request)
        {
            StringValues data;
            bool existQuery = request.Query.TryGetValue("data", out data);
            string dataValue = existQuery ? data.FirstOrDefault() : "No Value";

            StringValues e;
            bool IsE = request.Query.TryGetValue("e", out e);
            string eValue = IsE ? e.FirstOrDefault() : "No Value";

            string dataOut;

            if (eValue == "0")
            {
                dataOut = "Not Encode";
            }
            else
            {
                dataOut = HtmlEncoder.Default.Encode(dataValue);
            }
            string encoding_huongdan = File.ReadAllText("wwwroot/Html/encoding_huongdan.html");
            return dataOut.HtmlTag("div", "alert alert-danger") + encoding_huongdan;
        }

        public static string Gen_Cookies(HttpRequest request, HttpResponse response)
        {
            string tb = "";
            Console.WriteLine(request.Path);
            switch (request.Path)
            {
                case "/Cookies/read":
                    var listCookies = request.Cookies.Select(cookie =>
                    {
                        string s = $"{cookie.Key} - {cookie.Value}".HtmlTag("li");
                        return s;
                    });

                    tb = string.Join("", listCookies).HtmlTag("ul");

                    break;

                case "/Cookies/write":
                    response.Cookies.Append("id", "12345", new CookieOptions
                    {
                        //only active in this url
                        Path = "/",
                        //1 day
                        Expires = DateTime.Now.AddDays(1)
                    });
                    tb = "Saved Cookie - id - 12345 - expires 1 days".HtmlTag("div", "alert alert-danger");

                    break;
            }

            string huongdan = "<div class=\"list-group\">\r\n      <a class=\"list-group-item\" href=\"/Cookies/read\">Đọc Cookies</a>\r\n      <a class=\"list-group-item\" href=\"/Cookies/write\">Ghi Cookies</a>\r\n  </div>";


            return tb + huongdan;
        }

        public static async Task<string> GenAndHandle_Form(HttpRequest request)
        {
            string? name = "";
            bool choice = false;
            string? email = "";
            string? password = "";
            string? announce = "";

            if (request.Method == "POST")
            {
                var _form = request.Form;

                name = _form["hovaten"].FirstOrDefault() ?? "";
                email = _form["email"].FirstOrDefault() ?? "";
                password = _form["password"].FirstOrDefault() ?? "";
                choice = _form["luachon"].FirstOrDefault() == "on";
                announce = $@"Dữ liệu post - email: {email}
                          - hovaten: {name} - password: {password}
                          - luachon: {choice} ";
                if (_form.Files.Count > 0)
                {
                    var fileBasePath = "wwwroot/UploadFile/";
                    if (!Directory.Exists(fileBasePath)) Directory.CreateDirectory(fileBasePath);

                    string fileAnnounce = "Các file đã upload: ";
                    foreach (var formfile in _form.Files)
                    {
                        var filePath = fileBasePath + formfile.FileName;
                        fileAnnounce += $"{filePath} : {formfile.Length} bytes";

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formfile.CopyToAsync(stream);
                        }
                    }

                    announce += $"<br> {fileAnnounce}";
                }
            }

            var file = await File.ReadAllTextAsync("wwwroot/Html/form.html");
            return file.HtmlTag("div", "container") + announce;
        }
    }
}
