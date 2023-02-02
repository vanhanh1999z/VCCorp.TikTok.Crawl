using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCorp.TikTokCrawler.Model
{
    public class TikTokHastagCheckDTO
    {
        public string hashtag { get; set; }
        public string url_ids { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;

        public List<string> GetIdsVideo()
        {
            var ids = this.url_ids.Split('#');
            if (ids.ToList().Count > 0)
            {
                return ids.ToList();
            }
            return null;
        }

        public bool IsHashTagValid(string videoId)
        {
            return this.url_ids.Contains(videoId);
        }

        public List<TikTokHastagCheckDTO> DataTableToList(DataTable dt)
        {
            if (dt == null)
            {
                return null;
            }
            var videos = dt.AsEnumerable().Select(el => new TikTokHastagCheckDTO()
            {
                hashtag = el.Field<string>("hashtag"),
                url_ids = el.Field<string>("url_ids"),
                created_at = el.Field<DateTime>("created_at"),
                updated_at = el.Field<DateTime>("updated_at")
            }).ToList();
            return videos;
        }

        public string SqlInsert()
        {
            return @"insert into tiktok_hashtag_check ( hashtag, url_ids, created_at, updated_at ) values ( @hashtag, @url_ids, @created_at, @updated_at )";
        }
        public string SqlSelect(string hastag)
        {
            return @"SELECT hashtag,url_ids,created_at,updated_at FROM tiktok_hashtag_check";
        }
    }
}
