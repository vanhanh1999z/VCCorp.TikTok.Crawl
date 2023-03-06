using Crwal.Core.Log;
using K4os.Compression.LZ4;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCCorp.TikTokCrawler.Model;
using Newtonsoft.Json;
using Crwal.Core.Base;
using System.Reflection;

namespace VCCorp.TikTokCrawler
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string appName = Assembly.GetCallingAssembly().GetName().Name;
            Logging.Init(appName);
            try
            {
                Program.Init();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Logging.Infomation("--- Khởi tạo ứng dụng thành công ---");
                var f1 = new Form1();
                TiktokRuntime.TiktokForm = f1;
                Application.Run(f1);
                Logging.Infomation("--- Kết thúc ---");
            }
            catch (Exception ex)
            {
                Logging.Error(ex); ;
            }
        }
        public static async void Init()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "system.hcf");
                if (File.Exists(filePath))
                {
                    using (StreamReader r = new StreamReader(filePath))
                    {
                        var jsonString = r.ReadToEnd();
                        var config = JsonConvert.DeserializeObject<TiktokSystemDTO>(jsonString);
                        Program.LoadConfig(config);
                    }
                }
                else
                {
                    await Logging.ErrorAsync("Không tìm thấy file cấu hình");
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }
        public static async void LoadConfig(TiktokSystemDTO config)
        {
            try
            {
                TiktokRuntime.Config = new TiktokSystemDTO();
                if (config != null)
                {
                    TiktokRuntime.Config.ConfigSystem = new ConfigSystem()
                    {
                        KafkaConfig = new KafkaConfig()
                        {
                            SeverLink = config.ConfigSystem.KafkaConfig.SeverLink,
                            TopicTikTokPost = config.ConfigSystem.KafkaConfig.TopicTikTokPost
                        },
                        Platform = config.ConfigSystem.Platform,
                        Status = config.ConfigSystem.Status,
                    };
                    TiktokRuntime.Config.DbConnection = new DbConnection()
                    {
                        ConnectionToTableLinkProduct = config.DbConnection.ConnectionToTableLinkProduct,
                        ConnectionToTableReportDaily = config.DbConnection.ConnectionToTableReportDaily,
                        ConnectionToTableSiPost = config.DbConnection.ConnectionToTableSiPost,
                        IdTeleGramBotGroupCommentEco = config.DbConnection.IdTeleGramBotGroupCommentEco,
                        KeyBot = config.DbConnection.KeyBot
                    };
                }
                Logging.Infomation("Load cấu hình hệ thống thành công");
                //Logging.Infomation(TiktokRuntime.Config.ToJson());
            }
            catch (Exception ex)
            {
                await Logging.ErrorAsync("Load cấu hình hệ thống thất bại");
                Logging.Error(ex);
                throw;
            }
        }
    }
}
