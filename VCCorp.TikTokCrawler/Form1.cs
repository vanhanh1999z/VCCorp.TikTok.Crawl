using CefSharp;
using CefSharp.WinForms;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCCorp.TikTokCrawler.Controller;
using Crwal.Core.Log;
using System.Runtime.InteropServices;
using VCCorp.TikTokCrawler.Model;
using VCCorp.TikTokCrawler.DAO;

namespace VCCorp.TikTokCrawler
{
    public partial class Form1 : Form
    {
        private static ChromiumWebBrowser _browser = null;
        TikTokHashTagController tiktokHashtagController;
        public Form1()
        {
            InitializeComponent();
            InitBrowser("https://www.tiktok.com/");
            tiktokHashtagController = new TikTokHashTagController(_browser);
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                InitBrowser("https://www.tiktok.com/");
                await Task.Delay(10_000);
                TiktokPostController tiktokPostController = new TiktokPostController(_browser);
                await tiktokPostController.CrawlData();
            }
            catch (Exception)
            {
            }
            await Task.Delay(TimeSpan.FromHours(1));
        }
        public void InitBrowser(string urlBase)
        {
            if (_browser == null)
            {
                //this.WindowState = FormWindowState.Maximized;
                CefSettings s = new CefSettings();
                Cef.Initialize(s);
                _browser = new ChromiumWebBrowser(urlBase);
                this.panel1.Controls.Add(_browser);
                _browser.Dock = DockStyle.Fill;
            }
        }
        private async void rjButton1_Click(object sender, EventArgs e)
        {
            try
            {
                TikTokHashtagCheckDAO tagDAO = new TikTokHashtagCheckDAO();
                string lasTag = await tagDAO.GetLastTag();
                if (!String.IsNullOrEmpty(lasTag))
                {
                    await tiktokHashtagController.ResumeCrawl(lasTag);
                }
            }
            catch (Exception ex)
            {

                Logging.Error(ex);
            }
        }

        private async void btResume_Click(object sender, EventArgs e)
        {
            await tiktokHashtagController.ResumeCrawl(TiktokRuntime.IdxR);
        }

        private async void btStartedTag_Click(object sender, EventArgs e)
        {
            try
            {
                await tiktokHashtagController.CrawlData();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Form_1 - btStartedTag_Click");
            }
        }

        private async void btKingLive_Click(object sender, EventArgs e)
        {
            try
            {
                InitBrowser("https://www.tiktok.com/");
                await Task.Delay(10_000);
                //TiktokPostController tiktokPostController = new TiktokPostController(_browser);
                await tiktokHashtagController.CrawlData();

            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void SetLabelText(string text)
        {
            txtUrl.Text = text;
        }

        private void btShowDevTool_Click(object sender, EventArgs e)
        {
            _browser.ShowDevTools();
        }
    }
}
