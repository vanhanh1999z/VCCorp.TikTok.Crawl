using CefSharp;
using CefSharp.WinForms;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VCCorp.TikTokCrawler.Common
{
    public class Utilities
    {
        /// <summary>
        /// Mã hóa theo thuật toán MD5
        /// </summary>
        /// <param name="strEnc">chuỗi ký tự cần mã hóa theo MD5</param>
        /// <returns></returns>
        public static string Md5Encode(string strEnc)
        {
            try
            {
                // Tạo một thể hiện của MD5
                MD5 md5Hasher = MD5.Create();

                // Chuyển chuỗi sang dạng mảng mã hóa
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(strEnc));

                // Tạo đối tượng sBuilder để dùng phương thức nối chuỗi đạng byte
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Trả về chuỗi mã hóa theo dạng chuỗi hexadecimal
                return sBuilder.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Lấy phần tử đầu tiên thỏa mãn chuỗi pattern Regex
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pattern"></param>
        /// <returns>Trả ra chuỗi thỏa mãn pattern. trả ra rỗng nếu không thỏa mãn</returns>
        public static string Regex_GetItemFirst(string html, string pattern)
        {
            try
            {
                foreach (Match m in Regex.Matches(html, pattern, RegexOptions.IgnoreCase))
                {
                    return m.Value;
                }
            }
            catch { }
            return "";
        }

        /// <summary>
        /// Lấy phần tử cuối cùng thỏa mãn chuỗi pattern Regex
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string Regex_GetItemLast(string html, string pattern)
        {
            try
            {
                MatchCollection coll = Regex.Matches(html, pattern, RegexOptions.IgnoreCase);

                if (coll.Count > 0)
                {
                    return coll[coll.Count - 1].Value;
                }
            }
            catch { throw; }
            return "";
        }

        public static List<string> Regex_GetListItem(string html, string pattern)
        {
            List<string> ls = new List<string>();
            try
            {
                foreach (Match m in Regex.Matches(html, pattern, RegexOptions.IgnoreCase))
                {
                    ls.Add(m.Value);
                }
            }
            catch { }
            return ls;
        }

        public static string Regex_RemoveHtml(string html, string pattern)
        {
            try
            {
                return Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase);

            }
            catch { }
            return "";
        }

        /// <summary>
        /// Lấy đoạn Text (nếu cho phép xóa the <..>) hoặc lấy đoạn Html từ 1 đoạn HTML truyền vào, thảo mãn 3 điều kiện, mỗi điều kiện là 1 pattern Regex
        /// </summary>
        /// <param name="html">đoạn Html</param>
        /// <param name="pattern">pattern đầu tiên - dùng để giới hạn lại đoạn Html cần lấy</param>
        /// <param name="patternSub">pattern thứ 2 - dùng để giới hạn lại đoạn Html cần lấy từ đoạn 1</param>
        /// <param name="patternRemoveTag">pattern thứ 3 - xóa đi các thẻ không sử dụng</param>
        /// <param name="isRemoveAllTag">có xóa các thẻ Html để giữ lại là text không - true là có , false không</param>
        /// <returns></returns>
        public static string Regex_GetTextFromHtml(string html, string pattern, string patternSub, string patternRemoveTag, bool isRemoveAllTag)
        {
            string text = "";

            try
            {
                if (!string.IsNullOrEmpty(pattern))
                {
                    text = Regex_GetItemFirst(html, pattern);
                    if (string.IsNullOrEmpty(text))
                    {
                        return "";
                    }

                    if (!string.IsNullOrEmpty(patternSub)) // lấy subject bằng biểu thức thứ 2 sau khi được giới hạn bởi biểu thức 1
                    {
                        text = Regex_GetItemFirst(text, patternSub);
                        if (string.IsNullOrEmpty(text)) // chạy đoạn 2 có dữ liệu thì gán vào
                        {
                            return "";
                        }
                    }

                    if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(patternRemoveTag)) // xóa đi các thẻ không sử dụng
                    {
                        text = Regex.Replace(text, patternRemoveTag, "", RegexOptions.IgnoreCase);
                    }
                }
                else // không truyền vào pattern thì xóa sạch luôn
                {
                    text = html;
                    if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(patternRemoveTag)) // xóa đi các thẻ không sử dụng
                    {
                        text = Regex.Replace(text, patternRemoveTag, "", RegexOptions.IgnoreCase);
                    }
                }

                if (isRemoveAllTag == true)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        text = Regex.Replace(text, @"(<!--)(.*?)(-->)|(<script)(.*?)(script>)|(<style)(.*?)(style>)|(<iframe)(.*?)(iframe>)|(<embed)(.*?)(embed>)|(<object)(.*?)(object>)|(<video)(.*?)(video>)", " ", RegexOptions.IgnoreCase); // xóa cac the khong can thiet (neu co)
                        text = Regex.Replace(text, @"<(.*?)>", " ", RegexOptions.IgnoreCase); // xóa cặp thẻ
                        text = Regex.Replace(text, @"[\s]{2,}", " ", RegexOptions.IgnoreCase).Trim(); // nếu có nhiều hơn 2 khoảng trắng thì thay bằng 1 khoảng trắng
                    }
                }

            }
            catch { }

            return text.Trim();
        }

        /// <summary>
        /// Lấy đoạn html theo đoạn xpath, nếu có n đoạn thảo mãn thì sẽ nối chúng thành 1 đoạn
        /// </summary>
        /// <param name="html">html truyền vào</param>
        /// <param name="xpath">đoạn xpath, ví dụ //h1[contains(@class, 'title')]</param>
        /// <param name="isRemoveAllTag">có xóa hết các thẻ đánh dấu chỉ giữ lại đoạn text thôi</param>
        /// <returns></returns>
        public static string Xpath_GetHTML(string html, string xpath, bool isRemoveAllTag)
        {
            string htmlReturn = "";

            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                htmlDoc.OptionCheckSyntax = false;
                htmlDoc.OptionAutoCloseOnEnd = true;
                htmlDoc.LoadHtml(html); //responseString là đoạn html mà bạn lấy được ở phần trước.

                var node = htmlDoc.DocumentNode.SelectNodes(xpath); //div[....]               

                if (node != null && node.Count > 0)
                {
                    for (int i = 0; i < node.Count; i++)
                    {
                        htmlReturn += node[i].OuterHtml + " ";
                    }

                    // có xóa hết các thẻ - chừa lại toàn text
                    if (isRemoveAllTag == true)
                    {
                        htmlReturn = Regex.Replace(htmlReturn, @"(<!--)(.*?)(-->)|(<script)(.*?)(script>)|(<style)(.*?)(style>)|(<iframe)(.*?)(iframe>)|(<embed)(.*?)(embed>)|(<object)(.*?)(object>)|(<video)(.*?)(video>)", " ", RegexOptions.IgnoreCase); // xóa cac the khong can thiet (neu co)
                        htmlReturn = Regex.Replace(htmlReturn, @"<(.*?)>", " ", RegexOptions.IgnoreCase); // xóa cặp thẻ
                        htmlReturn = Regex.Replace(htmlReturn, @"[\s]{2,}", " ", RegexOptions.IgnoreCase).Trim(); // nếu có nhiều hơn 2 khoảng trắng thì thay bằng 1 khoảng trắng
                    }
                }
            }
            catch (Exception ex) { htmlReturn = "error, ex: " + ex.Message; }

            return htmlReturn;
        }

        /// <summary>
        /// Lấy danh sách các đoạn html. mỗi phần tử là 1 đoạn html thỏa mãn xpath
        /// </summary>
        /// <param name="html"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static List<string> Xpath_GetListHTML(string html, string xpath)
        {
            List<string> lis = new List<string>();

            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                htmlDoc.OptionCheckSyntax = false;
                htmlDoc.OptionAutoCloseOnEnd = true;
                htmlDoc.LoadHtml(html); //responseString là đoạn html mà bạn lấy được ở phần trước.

                var node = htmlDoc.DocumentNode.SelectNodes(xpath);
                if (node != null && node.Count > 0)
                {
                    for (int i = 0; i < node.Count; i++)
                    {
                        lis.Add(node[i].OuterHtml);
                    }
                }
            }
            catch (Exception ex) { }

            return lis;
        }

        /// <summary>
        /// Xóa đi 1 đoạn html theo xpath
        /// </summary>
        /// <param name="html"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static string Xpath_RemoveHtml(string html, string xpath)
        {
            string sourceReturn = html;
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDocRem = new HtmlAgilityPack.HtmlDocument();
                htmlDocRem.OptionFixNestedTags = true;
                htmlDocRem.OptionCheckSyntax = false;
                htmlDocRem.OptionAutoCloseOnEnd = true;
                htmlDocRem.LoadHtml(html);

                var nodeRem = htmlDocRem.DocumentNode.SelectNodes(xpath);
                if (nodeRem != null && nodeRem.Count > 0)
                {
                    foreach (HtmlNode n in nodeRem)
                    {
                        n.Remove();
                    }
                }

                // gán lại cho nó xóa lần tiếp
                sourceReturn = htmlDocRem.DocumentNode.OuterHtml;
            }
            catch (Exception ex) { }

            return sourceReturn;
        }

        /// <summary>
        /// Lấy nguồn từ 1 Url. Ex: https://vnexpress.net/gia-thuc-pham-tang-gap-ba-4306050.html -> vnexpress.net
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetSourceFromUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string source = Regex.Replace(url, @"http:\/\/|https:\/\/|www(.*?).", ""); // xóa đầu http hay https
                source = Regex.Replace(source, @"(\/|\?)([\s\S]+)", ""); // xóa đuôi nếu cuối cùng có dấu /

                string[] arr = source.Split('/');
                if (arr.Length > 0)
                {
                    source = arr[0];
                }

                return source;
            }

            return "";
        }

        /// <summary>
        /// Lấy domain từ 1 url ex: https://vnexpress.net/gia-thuc-pham-tang-gap-ba-4306050.html -> https://vnexpress.net
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDomainFromUrl(string url)
        {
            Uri uri = new Uri(url);
            return uri.Scheme + "://" + uri.Host.ToString();
        }

        /// <summary>
        /// Chuyển đổi Unix timestamp to DateTime
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(double unixTimeStamp)
        {
            //unixTimeStamp = 1537149600000;
            System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
            return dtDateTime;

            // trả ra: 9/17/2018 2:00:00 AM
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        /// <summary>
        /// Lấy danh sách file hình từ 1 đoạn HTML
        /// </summary>
        /// <param name="htmlImage"></param>
        /// <returns></returns>
        public static List<string> GetListImgFromHTML(string htmlImage)
        {
            List<string> lisReturn = new List<string>();

            List<string> tagImg = Regex_GetListItem(htmlImage, "<img.*?>");

            if (tagImg != null && tagImg.Count > 0)
            {
                foreach (string img in tagImg)
                {
                    string filePath = "";
                    if (img.ToLower().Contains("src"))
                    {
                        filePath = Regex_GetTextFromHtml(img, "(src[\\s]{0,}=[\\s]{0,}('|\")).*?('|\")", "", "(src[\\s]{0,}=[\\s]{0,}('|\"))|('|\")", true);
                    }

                    if (filePath.ToLower().Contains("data:image"))
                    {
                        filePath = FindImageFromBase64(img);
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            lisReturn.Add(filePath);
                        }
                    }
                    else
                    {
                        lisReturn.Add(filePath);
                    }

                }

            }

            return lisReturn;
        }

        /// <summary>
        /// tìm đường dẫn hình ảnh trong 1 file base64
        /// </summary>
        /// <param name="htmlImage"></param>
        /// <returns></returns>
        public static string FindImageFromBase64(string htmlImage)
        {

            string tagImg = Regex_GetItemFirst(htmlImage, "<img.*?>");

            if (!string.IsNullOrEmpty(tagImg))
            {
                List<string> lisAtt = Regex_GetListItem(tagImg, "('|\").*?('|\")");
                if (lisAtt != null && lisAtt.Count > 0)
                {
                    string img = "";
                    foreach (string att in lisAtt)
                    {
                        if (att.ToLower().Contains("data:image"))
                        {
                            continue;
                        }

                        if (att.ToLower().Contains(".jpg") || att.ToLower().Contains(".gif") || att.ToLower().Contains(".png") || att.ToLower().Contains(".jpeg"))
                        {
                            return img = Regex.Replace(att, "('|\")|('|\")", "").Trim();
                        }
                    }
                }

            }

            return "";
        }

        public static string FixPathImage(string src, string domain)
        {
            try
            {
                if (string.IsNullOrEmpty(src))
                {
                    return "";
                }

                if (src.StartsWith("http")) // bắt đầu với htttp
                {
                    return src;
                }
                else
                {
                    if (src.StartsWith("//")) // //img.giaoduc.net.vn/w1050/uploaded/2019/aslyefjpeag/2019_11_03/5c4fbc86ce3b6cdd18774a99a3371094.jpg
                    {
                        // thiếu nghi thức
                        if (domain.StartsWith("https"))
                        {
                            return "https:" + src;
                        }
                        else
                        {
                            return "http:" + src;
                        }
                    }

                    if (src.StartsWith("/")) // bắt đầu bằng dấu
                    {
                        return domain + src;
                    }



                    if (src.StartsWith("./")) // bắt đầu bằng dấu ./
                    {
                        return domain + src.Remove(0, 1);
                    }

                    if (src.StartsWith("../")) // bắt đầu bằng dấu ../
                    {
                        return domain + src.Remove(0, 2);
                    }

                    if (src.StartsWith("../../")) // bắt đầu bằng dấu ../../
                    {
                        return domain + src.Remove(0, 5);
                    }

                    // trường hợp không có dấu gì đằng trước
                    return domain + "/" + src;
                }
            }
            catch { }

            return "";
        }

        public static string FixPathURL(string url, string domain)
        {
            try
            {
                if (url.StartsWith("http://") || url.StartsWith("HTTP://") || url.StartsWith("https://") || url.StartsWith("HTTPS://")) // nó bắt đầu bằng http rồi, chứng tỏ link tuyệt đối
                {
                    return url;
                }

                if (!url.Contains(domain)) // không chứa đựng domain
                {
                    if (url.StartsWith("//"))
                    {
                        // chỉ lấy schema
                        bool isHttps = false;
                        if (domain.StartsWith("https"))
                        {
                            isHttps = true;
                        }

                        if (isHttps == true)
                        {
                            url = "https:" + url;
                        }
                        else
                        {
                            url = "http:" + url;
                        }

                        return url;
                    }

                    if (url.StartsWith("/")) // bắt đầu có dấu / rồi
                    {
                        url = domain + url;
                        return url;
                    }

                    if (url.StartsWith("./"))
                    {
                        url = domain + url.Remove(0, 1); // xóa đi dấu chấm ở đầu
                        return url;
                    }

                    if (url.StartsWith("../../")) // ../../
                    {
                        url = domain + url.Remove(0, 5); // xóa đi dấu chấm ở đầu
                        return url;
                    }

                    if (url.StartsWith("../"))
                    {
                        url = domain + url.Remove(0, 2); // xóa đi dấu chấm ở đầu
                        return url;
                    }

                    if (url.StartsWith("~/"))
                    {
                        url = domain + url.Remove(0, 1); // xóa đi dấu ~ ở đầu
                        return url;
                    }

                    url = domain + "/" + url; // không domain và không có dấu đặc biệt đằng trước

                }
                return url;
            }
            catch (Exception ex) { }
            return url;
        }

        public static bool CheckUrlInsideWebsite(string url, string domain)
        {
            // xóa đi cái đầu http của cả 2
            string urlRemoved = url.Replace("https://", "").Replace("http://", "").Replace("www.", "");
            string domainRemoved = domain.Replace("https://", "").Replace("http://", "").Replace("www.", "");

            if (urlRemoved.StartsWith(domainRemoved))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// xóa dấu có thêm vào dấu -
        /// </summary>
        /// <param name="str_input"></param>
        /// <returns></returns>
        public static string ReplaceUnicodeWithLine(string str_input)
        {
            try
            {
                #region replace a
                string part_a = @"[á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ]";
                str_input = Regex.Replace(str_input, part_a, "a", RegexOptions.IgnoreCase);

                #endregion

                #region replace e
                string part_e = @"[é|è|ẹ|ẻ|ẽ|ê|ế|ề|ệ|ể|ễ]";
                str_input = Regex.Replace(str_input, part_e, "e", RegexOptions.IgnoreCase);
                #endregion

                #region replace o
                string part_o = @"[ó|ò|ỏ|õ|ọ|ơ|ớ|ờ|ở|ỡ|ợ|ô|ố|ồ|ổ|ỗ|ộ|ố]"; // chữ o thường ò
                str_input = Regex.Replace(str_input, part_o, "o", RegexOptions.IgnoreCase);
                #endregion

                #region replace u
                string part_u = @"[ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự]";
                str_input = Regex.Replace(str_input, part_u, "u", RegexOptions.IgnoreCase);
                #endregion

                #region replace i
                string part_i = @"[í|ì|ĩ|ị|ỉ]";
                str_input = Regex.Replace(str_input, part_i, "i", RegexOptions.IgnoreCase);
                #endregion

                #region replace d
                string part_d = @"[đ]";
                str_input = Regex.Replace(str_input, part_d, "d", RegexOptions.IgnoreCase);
                #endregion

                #region replace y
                string part_y = @"[ý|ỳ|ỷ|ỹ|ỵ]";
                str_input = Regex.Replace(str_input, part_y, "y", RegexOptions.IgnoreCase);
                #endregion

                #region replace ký tự đặc biệt
                string partSpec = @"[!|@|#|=|$|%|^|&|*|\(|\)|+|\\|\[\]|{|}|:|;|\?|<|>|\.|,]";
                str_input = Regex.Replace(str_input, partSpec, "");
                #endregion

                #region có nhiều khoảng trắng - 1 khoảng trắng
                string partMulSpace = @"[\s]{2,}";
                str_input = Regex.Replace(str_input, partMulSpace, " ").Trim();
                #endregion

                #region khỏang trắng thành dấu _
                string partSpace = @"[\s]";
                str_input = Regex.Replace(str_input, partSpace, "-");

                str_input = str_input.Replace('/', '-');
                #endregion

                #region nhiều gạch thành 1 gạch _
                string partLine = @"[-]{2,}";
                str_input = Regex.Replace(str_input, partLine, "-");
                #endregion
            }
            catch
            {
                str_input = "";
            }

            str_input = str_input.ToLower();

            return str_input;
        }
        public static async Task<string> GetBrowserSource(ChromiumWebBrowser browser)
        {
            return await browser.GetMainFrame().GetSourceAsync();
        }
        public static readonly JsonSerializerOptions opt = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };
        public static string RemoveSpecialCharacter(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return "";
            }

            return Regex.Replace(text, @"\t|\n|\r|\s+", " ");
        }
        public static async Task<string> EvaluateJavaScriptSync(string jsScript, ChromiumWebBrowser browser)
        {
            string jsonFromJS = null;
            if (browser.CanExecuteJavascriptInMainFrame && !String.IsNullOrEmpty(jsScript))
            {
                JavascriptResponse result = await browser.EvaluateScriptAsync(jsScript);

                if (result.Success)
                {
                    jsonFromJS = "";

                    if (result.Result != null)
                    {
                        jsonFromJS = result.Result.ToString();
                    }
                }
            }
            return jsonFromJS;
        }
    }
}
