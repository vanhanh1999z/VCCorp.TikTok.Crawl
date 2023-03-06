using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCorp.TikTokCrawler.Model
{
    public class TikTokDTO
    {
        public int si_demand_source_id { get; set; }
        public string post_id { get; set; }
        public string platform { get; set; } = "tiktok";
        public string link { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public DateTime crawled_time { get; set; }
        public int status { get; set; }
        public string domain { get; set; }
        public int status_link { get; set; }
        public int total_comment { get; set; }
        public string hashtag { get; set; }
        public string user_crawler { get; set; } = "Hanhlv";
        public DateTime insert_time { get; set; } = DateTime.Now;
        public int index_slave { get; set; } = 0;
        public TikTokDTO(string post_id, DateTime create_time)
        {
            this.post_id = post_id;
            this.create_time = create_time;
        }
        public TikTokDTO(string hashTagFile)
        {
            this.hashtag = hashTagFile;
        }
        public TikTokDTO()
        { }

    }
}
