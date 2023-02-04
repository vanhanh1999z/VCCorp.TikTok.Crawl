using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VCCorp.TikTokCrawler.Model
{
    public static class TiktokRuntime
    {
        public static TiktokSystemDTO Config;
        public static int IdxR;
        public static List<string> Tags;
        public static Form1 TiktokForm;
    }
    public class TiktokSystemDTO
    {
        public ConfigSystem ConfigSystem { get; set; }
        public DbConnection DbConnection { get; set; }
    }
    public class KafkaConfig
    {
        public string SeverLink { get; set; }
        public string TopicTikTokPost { get; set; }
    }
    public class ConfigSystem
    {
        public int Status { get; set; }
        public string Platform { get; set; }
        public KafkaConfig KafkaConfig { get; set; }
    }
    public class DbConnection
    {
        public string ConnectionToTableLinkProduct { get; set; }
        public string ConnectionToTableSiPost { get; set; }
        public string ConnectionToTableReportDaily { get; set; }
        public string KeyBot { get; set; }
        public long IdTeleGramBotGroupCommentEco { get; set; }
    }
}
