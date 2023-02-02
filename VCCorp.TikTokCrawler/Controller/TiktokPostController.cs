using CefSharp.WinForms;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VCCorp.TikTokCrawler.Common;
using VCCorp.TikTokCrawler.DAO;
using VCCorp.TikTokCrawler.Model;

namespace VCCorp.TikTokCrawler.Controller
{
    public class TiktokPostController
    {
        private ChromiumWebBrowser _browser = null;
        private readonly HtmlAgilityPack.HtmlDocument _document = new HtmlAgilityPack.HtmlDocument();
        private string URL_KINGLIVE = "https://www.tiktok.com/@kinglive.vn";
        private const string _jsAutoScroll = @"window.scrollTo(0, document.body.scrollHeight)/3";
        private string path = "D:\\Test\\LastDatePost.txt";

        public TiktokPostController(ChromiumWebBrowser browser)
        {
            _browser = browser;
        }

        public async Task CrawlData()
        {
            if (!File.Exists(path))
            {
                File.Create(path);

            }
            //await NewLinkTiTok_Db(URL_KINGLIVE);
            await NewDetailByUrl_Db();
        }

        /// <summary>
        /// Thêm url vào tiktok_link table. (Bảng local)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<TikTokDTO>> NewLinkTiTok_Db(string url)
        {
            List<TikTokDTO> tiktokPost = new List<TikTokDTO>();
            ushort indexLastContent = 0;
            try
            {
                await _browser.LoadUrlAsync(url);
                await Task.Delay(10_000);
                byte i = 0;
                while (i < 3)
                {
                    i++;
                    string html = await Utilities.GetBrowserSource(_browser).ConfigureAwait(false);
                    _document.LoadHtml(html);
                    html = null;

                    HtmlNodeCollection divComment = _document.DocumentNode.SelectNodes($"//div[contains(@class,'tiktok-x6y88p-DivItemContainerV2')][position()>{indexLastContent}]");
                    if (divComment == null)
                    {
                        break;
                    }
                    if (divComment != null)
                    {
                        foreach (HtmlNode item in divComment)
                        {
                            string urlVid = item.SelectSingleNode(".//div[contains(@class,'tiktok-yz6ijl-DivWrapper')]/a")?.Attributes["href"].Value;
                            string idVid = Regex.Match(urlVid, @"(?<=/video/)\d+").Value; // lấy id_post

                            TikTokDTO content = new TikTokDTO();
                            content.link = urlVid;
                            content.domain = URL_KINGLIVE;
                            content.post_id = idVid;
                            content.status_link = 0;

                            tiktokPost.Add(content);

                            //Trường hợp chưa có file lưu max post_id
                            if (new FileInfo(path).Length == 0)
                            {
                                TikTokPostDAO msql = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableLinkProduct);
                                await msql.InserTikTokLinkTable(content);
                                msql.Dispose();
                            }
                            else
                            {
                                string contentFile = File.ReadAllText(path);
                                string[] items = contentFile.Split('|');
                                TikTokDTO getTextFile = new TikTokDTO(items[0], DateTime.Parse(items[1]));//lấy object trong file .txt
                                string postid = getTextFile.post_id;
                                if (double.Parse(postid) < double.Parse(idVid))//lấy id lớn hơn
                                {
                                    TikTokPostDAO msql = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableLinkProduct);
                                    await msql.InserTikTokLinkTable(content);
                                    msql.Dispose();
                                }
                            }
                            indexLastContent++;
                        }
                    }
                    //check JS roll xuống cuối trang
                    string checkJs = await Common.Utilities.EvaluateJavaScriptSync(_jsAutoScroll, _browser).ConfigureAwait(false);
                    if (checkJs == null)
                    {
                        break;
                    }
                    await Task.Delay(10_000);
                }
            }
            catch { }
            return tiktokPost;
        }

        /// <summary>
        /// Lấy details từ url bảng tiktok_link.(Lưu vào bảng chính)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<TikTokDTO>> NewDetailByUrl_Db()
        {
            List<TikTokDTO> tiktokPost = new List<TikTokDTO>();
            try
            {   //lấy url trong db
                TikTokPostDAO contentDAO = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableLinkProduct);
                List<TikTokDTO> dataUrl = contentDAO.GetLinkByDomain(URL_KINGLIVE);
                contentDAO.Dispose();
                for (int i = 0; i < dataUrl.Count; i++)
                {
                    int status_link = dataUrl[i].status_link;
                    if (status_link == 0)// check xem đã bóc hay chưa?
                    {
                        string link = dataUrl[i].link; //lấy link từ db
                        string idVid = Regex.Match(link, @"(?<=/video/)\d+").Value; // lấy id_post

                        await _browser.LoadUrlAsync(link);
                        await Task.Delay(5_000);

                        string html = await Utilities.GetBrowserSource(_browser).ConfigureAwait(false);
                        _document.LoadHtml(html);
                        html = null;

                        TikTokDTO tikTokDTO = new TikTokDTO();
                        DateTime createDate = DateTime.Now;
                        string postDate = _document.DocumentNode.SelectSingleNode("//span[contains(@class,'e17fzhrb2')]/span[3]")?.InnerText;
                        if (!string.IsNullOrEmpty(postDate))
                        {
                            DateTimeFormatAgain dtFomat = new DateTimeFormatAgain();
                            string date = dtFomat.GetDateBySearchText(postDate, "yyyy-MM-dd HH:mm:ss");
                            try
                            {
                                createDate = Convert.ToDateTime(date);
                            }
                            catch { }
                        }
                        tikTokDTO.create_time = createDate; // ngày tạo vid
                        tikTokDTO.link = link; // link vid
                        tikTokDTO.post_id = idVid; //id video
                        tikTokDTO.platform = TiktokRuntime.Config.ConfigSystem.Platform;
                        tikTokDTO.crawled_time = DateTime.Now; // thời gian bóc
                        tikTokDTO.update_time = createDate; // thời gian update
                        tikTokDTO.status = TiktokRuntime.Config.ConfigSystem.Status;
                        tiktokPost.Add(tikTokDTO);

                        //Trường hợp chưa có file lưu max post_id, max create_date
                        if (new FileInfo(path).Length == 0)
                        {
                            if (DateTime.Now.AddDays(-7) < createDate)
                            {
                                //Bảng chính
                                TikTokPostDAO msql = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableSiPost);
                                await msql.InserToSiPostTable(tikTokDTO);
                                msql.Dispose();

                                ////Bảng db test
                                //TikTokPostDAO msql = new TikTokPostDAO(ConnectionDAO.ConnectionToTableLinkProduct);
                                //await msql.InserTikTokSourcePostTable(tikTokDTO);
                                //msql.Dispose();

                                //Update Status (crawled == 1 )
                                TikTokPostDAO msql1 = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableLinkProduct);
                                await msql1.UpdateStatus(link);
                                msql1.Dispose();
                            }
                        }
                        else
                        {
                            string content = File.ReadAllText(path);
                            string[] items = content.Split('|');
                            TikTokDTO getTextFile = new TikTokDTO(items[0], DateTime.Parse(items[1]));//lấy object trong file .txt
                            string postid = getTextFile.post_id;

                            if (double.Parse(postid) < double.Parse(idVid))//lấy id lớn hơn
                            {
                                //Bảng chính
                                TikTokPostDAO msql = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableSiPost);
                                await msql.InserToSiPostTable(tikTokDTO);
                                msql.Dispose();

                                ////Bảng db Test
                                //TikTokPostDAO msql = new TikTokPostDAO(ConnectionDAO.ConnectionToTableLinkProduct);
                                //await msql.InserTikTokSourcePostTable(tikTokDTO);
                                //msql.Dispose();

                                //Update Status (crawled == 1 )
                                TikTokPostDAO msql1 = new TikTokPostDAO(TiktokRuntime.Config.DbConnection.ConnectionToTableLinkProduct);
                                await msql1.UpdateStatus(link);
                                msql1.Dispose();
                            }
                        }
                    }
                }
                DateTime lastDate = tiktokPost.Max(x => x.create_time);//lấy thời gian post gần nhất (lớn nhất)               
                string post_id = tiktokPost.FirstOrDefault(x => x.create_time == lastDate)?.post_id ?? "";
                File.WriteAllText(path, post_id + "|" + lastDate.ToString("yyyy-MM-dd HH:mm:ss")); //lưu vào file
            }
            catch { }
            return tiktokPost;
        }
    }
}

