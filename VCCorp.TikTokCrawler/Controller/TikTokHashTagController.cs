using CefSharp.DevTools.Debugger;
using CefSharp.WinForms;
using Crwal.Core.Attribute;
using Crwal.Core.Base;
using Crwal.Core.Log;
using Crwal.Core.Sql;
using Google.Protobuf;
using HtmlAgilityPack;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCCorp.TikTokCrawler.Common;
using VCCorp.TikTokCrawler.DAO;
using VCCorp.TikTokCrawler.Model;


namespace VCCorp.TikTokCrawler.Controller
{
    public class TikTokHashTagController
    {
        private ChromiumWebBrowser _browser = null;
        private readonly HtmlAgilityPack.HtmlDocument _document = new HtmlAgilityPack.HtmlDocument();
        private string URL_TIKTOK = "https://www.tiktok.com/";
        private const string _jsLoadMore = @"document.getElementsByClassName('tiktok-154bc22-ButtonMore')[0].click()";
        private string path = "D:\\ListHashTag.txt";
        public string _currHashtag = "";
        public List<string> _tags = new List<string>();

        public TikTokHashTagController(ChromiumWebBrowser browser)
        {
            _browser = browser;
        }
        public TikTokHashTagController()
        {
        }
        public async Task CrawlData()
        {
            if (_tags == null || _tags.Count <= 0)
            {
                _tags = await GetHashtagAsync();

            }
            for (int i = 0; i < _tags.Count; i++)
            {
                _currHashtag = _tags[i];
                string Url = "https://www.tiktok.com/search?q=" + _tags[i];
                TiktokRuntime.TiktokForm.SetLabelText(Url);
                var result = await CrawlHashtag(Url);
                if (result.Count == 0 || result == null)
                {
                    await Logging.ErrorAsync("Vui lòng đăng nhập nick khác để tiếp tục crawl");
                    TiktokRuntime.IdxR = _tags.IndexOf(_currHashtag);
                    TiktokRuntime.Tags = _tags;
                    break;
                }
            }
            //await GetHashTagFromFile();
        }

        /// <summary>
        /// Lấy HashTag từ file txt
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task GetHashTagFromFile()
        {
            string[] lines = File.ReadAllLines(path);//đọc các dòng trong file .txt
            foreach (string line in lines)
            {
                TikTokDTO getTextFile = new TikTokDTO(line);//lấy object trong file .txt
                string url = getTextFile.hashtag;
                _currHashtag = url;
                await CrawlHashtag("https://www.tiktok.com/search?q=" + url);
            }
            Logging.Infomation("**************************************************");
            Logging.Infomation("      *************** HOÀN TẤT **************");
            Logging.Infomation("**************************************************");
        }
        public async Task ResumeCrawl(int idx)
        {
            if (_tags == null || _tags.Count <= 0)
            {
                _tags = await GetHashtagAsync();

            }
            for (int i = idx; i < _tags.Count; i++)
            {
                _currHashtag = _tags[i];
                string Url = "https://www.tiktok.com/search?q=" + _tags[i];
                TiktokRuntime.TiktokForm.SetLabelText(Url);
                var result = await CrawlHashtag(Url);
                if (result.Count == 0 || result == null)
                {
                    await Logging.ErrorAsync("Vui lòng đăng nhập nick khác để tiếp tục crawl");
                    break;
                }
            }
        }
        public async Task ResumeCrawl(string currentHashTag)
        {
            try
            {
                if (_tags == null || _tags.Count <= 0)
                {
                    _tags = await GetHashtagAsync();
                }
                int idx = _tags.IndexOf(currentHashTag);
                if (idx != -1)
                {
                    for (int i = idx; i < _tags.Count; i++)
                    {
                        _currHashtag = _tags[i];
                        string Url = "https://www.tiktok.com/search?q=" + _tags[i];
                        TiktokRuntime.TiktokForm.SetLabelText(Url);
                        var result = await CrawlHashtag(Url);
                        if (result.Count == 0 || result == null)
                        {
                            await Logging.ErrorAsync("Vui lòng đăng nhập nick khác để tiếp tục crawl");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

        }
        /// <summary>
        /// Lọc Hashtag từ table Si_Hashtag
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<string>> GetHashtagAsync()
        {
            TikTokPostDAO sql = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableSiPost);
            TikTokPostDAO sqlHashtag = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableLinkProduct);
            DateTime fromDate = DateTime.Now.AddDays(-2);
            DateTime toDate = DateTime.Now;
            DateTime currentDate = DateTime.Now.AddDays(-1);
            //Lấy hashtag từ db trong khoảng date
            //List<string> lstHashtag = sql.GetHashtagInTableSiHastag(fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));

            //Lấy hastag từ db trong chính xác 1 ngày
            List<string> lstHashtag = await sqlHashtag.GetHashtagInTableSiHastagByCurrentDate(currentDate.ToString("yyyy-MM-dd"));
            sql.Dispose();

            List<string> lstDuplicate = new List<string>();//list trùng
            //List<string> lstCheckDuplicate = new List<string>();//list không trùng        

            for (int i = 0; i < lstHashtag.Count; i++)
            {
                string rawHashtag = lstHashtag[i];//hashtag  ban đầu
                string hashtagRemoveSign = Common.Hastag_Helper.RemoveSignVietnameseString(rawHashtag); //hashtag sau khi loại bỏ dấu Vietnamese
                string hashtagRemoveSpace = Regex.Replace(hashtagRemoveSign, @"\s+", "");//hashtag sau khi loại bỏ dấu cách
                string hashtagRemoveChar = hashtagRemoveSpace.Replace(".", string.Empty).ToLower();// hashtag sau khi loại bỏ dấu chấm
                lstDuplicate.Add(hashtagRemoveChar);//lưu vào list trùng
            }

            //Loại bỏ bản ghi trùng lặp
            _tags = lstDuplicate.Distinct().ToList();

            //lấy từng hashtag trong list không trùng để bóc
            return _tags;
        }


        /// <summary>
        /// Bóc các bài viết HashTag. Lưu vào table si_demand_source_post và gửi Kafka ILS
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<TikTokDTO>> CrawlHashtag(string url)
        {
            List<TikTokDTO> tiktokPost = new List<TikTokDTO>();
            var hashtagCheck = new TikTokHashtagCheckDAO();
            ushort indexLastContent = 0;
            var videoIds = await hashtagCheck.SelectByHashtagAsync(_currHashtag);
            try
            {
                await _browser.LoadUrlAsync(url);
                Logging.Infomation("Load thành công url: " + url);
                await Task.Delay(6_000);
                byte i = 0;
                while (i < 100)
                {
                    i++;
                    string html = await Common.Utilities.GetBrowserSource(_browser).ConfigureAwait(false);
                    _document.LoadHtml(html);
                    html = null;

                    HtmlNodeCollection divComment = _document.DocumentNode.SelectNodes($"//div[contains(@class,'tiktok-1soki6-DivItemContainerForSearch')][position()>{indexLastContent}]");
                    if (divComment == null)
                    {
                        await Logging.ErrorAsync("Không tìm kiếm được nội dung: " + _currHashtag);
                        break;
                    }
                    if (divComment != null)
                    {
                        foreach (HtmlNode item in divComment)
                        {
                            //tối ưu playCount về dạng int
                            string numString = item.SelectSingleNode(".//strong[contains(@class,'tiktok-ws4x78-StrongVideoCount')]")?.InnerText;
                            string getChar = Regex.Match(numString, @"\D$").Value;// tách lấy chữ (ví dụ 10K,10M... thì lấy K, M)
                            string getFullNumAndChar = Regex.Match(numString, @".*\d+").Value;//tách lấy số và kí tự (dấu chấm)
                            string getOnlyNum = getFullNumAndChar.Replace(".", string.Empty).ToLower();//loại bỏ dấu chấm chỉ lấy số (ví dụ 21.9)
                            int convertNum = int.Parse(getOnlyNum.ToString());//convert sang int
                            string urlVid = item.SelectSingleNode(".//div[contains(@class,'tiktok-yz6ijl-DivWrapper')]/a")?.Attributes["href"].Value;
                            string idVid = Regex.Match(urlVid, @"(?<=/video/)\d+").Value; // lấy id_post

                            TikTokDTO content = new TikTokDTO();
                            DateTime createDate = DateTime.Now;
                            string postDate = item.SelectSingleNode(".//div[contains(@class,'tiktok-842lvj-DivTimeTag')]")?.InnerText;

                            if (!string.IsNullOrEmpty(postDate))
                            {
                                Common.DateTimeFormatAgain dtFomat = new Common.DateTimeFormatAgain();
                                string date = dtFomat.GetDateBySearchText(postDate, "yyyy-MM-dd HH:mm:ss");
                                try
                                {
                                    createDate = Convert.ToDateTime(date);
                                }
                                catch (Exception ex)
                                {
                                    //Logging.Error(ex);
                                }
                            }
                            content.link = urlVid;
                            content.create_time = createDate; // ngày tạo vid
                            content.post_id = idVid; //id video
                            content.platform = TiktokRuntime.Config.ConfigSystem.Platform;
                            content.crawled_time = DateTime.Now; // thời gian bóc
                            content.update_time = createDate; // thời gian update
                            content.status = TiktokRuntime.Config.ConfigSystem.Status;
                            content.total_comment = ConvertPlayCount(getChar, convertNum);//play count
                            content.hashtag = _currHashtag;
                            tiktokPost.Add(content);
                            //Lấy vid từ tháng 11
                            if (createDate > DateTime.Now.AddDays(-31))
                            {
                                TikTokPostDAO msql = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableSiPost);
                                Logging.Fatal(videoIds.ToJson());
                                if (videoIds == null || String.IsNullOrEmpty(videoIds.url_ids))
                                {
                                    Logging.Warning($"Hashtag {_currHashtag} chưa có video nào được crwal");
                                    await hashtagCheck.Insert(new TikTokHastagCheckDTO()
                                    {
                                        hashtag = _currHashtag,
                                        url_ids = "",
                                    });
                                    videoIds.url_ids = "";
                                    videoIds.hashtag = _currHashtag;
                                }
                                if (!videoIds.url_ids.Contains(idVid))
                                {
                                    Logging.Infomation("Bắt đầu thêm dữ liệu vào bảng tiktok_source_post");
                                    Logging.Warning("Bắt đầu cào video " + idVid);
                                    await msql.InserTikTokSourcePostTable(content);
                                    msql.Dispose();
                                    #region gửi đi cho ILS
                                    Tiktok_Post_Kafka_Model kafka = new Tiktok_Post_Kafka_Model();
                                    kafka.IdVideo = content.post_id;
                                    kafka.UserName = item.SelectSingleNode(".//p[contains(@class,'tiktok-2zn17v-PUniqueId etrd4pu6')]")?.InnerText;
                                    kafka.IdUser = "@" + kafka.UserName;
                                    kafka.UrlUser = URL_TIKTOK + "@" + kafka.UserName;
                                    kafka.Avatar = item.SelectSingleNode(".//span[contains(@class,'tiktok-tuohvl-SpanAvatarContainer')]//img")?.Attributes["src"]?.Value ?? "";
                                    kafka.Content = Common.Utilities.RemoveSpecialCharacter(item.SelectSingleNode(".//div[contains(@class,'tiktok-1ejylhp-DivContainer')]/span[contains(@class,'tiktok-j2a19r-SpanText')][1]")?.InnerText);
                                    kafka.LinkVideo = urlVid;
                                    kafka.PlayCounts = ConvertPlayCount(getChar, convertNum);
                                    kafka.TimePost = createDate;
                                    kafka.TimePostTimeStamp = (double)(Date_Helper.ConvertDateTimeToTimeStamp(createDate));
                                    kafka.TimeCreated = DateTime.Now;
                                    kafka.TimeCreateTimeStamp = (double)(Date_Helper.ConvertDateTimeToTimeStamp(DateTime.Now));
                                    string jsonPost = Kafka_Helper.ToJson<Tiktok_Post_Kafka_Model>(kafka);
                                    Kafka_Helper kh = new Kafka_Helper();
                                    await kh.InsertPost(jsonPost, "crawler-data-tiktok");
                                    #endregion
                                    videoIds.url_ids += "#" + idVid;
                                    await hashtagCheck.UpdateAsync(videoIds);
                                }
                                else
                                {
                                    Logging.Warning($"Video {idVid} đã được crwal");
                                }
                            }
                            indexLastContent++;
                        }
                    }
                    //check JS nút xem thêm
                    string checkJs = await Common.Utilities.EvaluateJavaScriptSync(_jsLoadMore, _browser).ConfigureAwait(false);
                    if (checkJs == null)
                    {
                        break;
                    }
                    await Task.Delay(6_000);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
            return tiktokPost;
        }
        public int ConvertPlayCount(string specChar, int playCount)
        {
            if (specChar == "K")
            {
                return playCount * 100;
            }
            else if (specChar == "M")
            {
                return playCount * 100000;
            }
            else if (specChar == "")
            {
                return playCount;
            }
            return playCount;
        }

    }
}
