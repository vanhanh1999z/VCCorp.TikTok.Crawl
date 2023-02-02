using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCorp.TikTokCrawler.Model
{
    public class Tiktok_Post_Kafka_Model
    {
        public string IdVideo { set; get; }
        public string UserName { set; get; }
        public string IdUser { set; get; }
        public string UrlUser { set; get; }
        public string Avatar { set; get; }
        public string Content { set; get; }
        public string LinkVideo { set; get; }
        public int Likes { set; get; }
        public int Comments { set; get; }
        public int Shares { set; get; }
        public int PlayCounts { set; get; }
        public int CommentCounts { set; get; }
        public double TimePostTimeStamp { set; get; }
        public DateTime TimePost { set; get; }
        public DateTime TimeCreated { set; get; }
        public double TimeCreateTimeStamp { set; get; }
        public int Followers { set; get; }
        public int Following { set; get; }
    }
}
