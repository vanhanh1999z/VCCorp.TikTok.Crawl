using Crwal.Core.Log;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using VCCorp.TikTokCrawler.Model;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VCCorp.TikTokCrawler.DAO
{
    public class TikTokPostDAO
    {
        private readonly MySqlConnection _conn;
        public TikTokPostDAO(string connection)
        {
            _conn = new MySqlConnection(connection);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    _conn.Close();
                    _conn.Dispose();
                }
                else
                {
                    _conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Insert Content to si_demand_resource_post table (bảng chính)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<int> InserToSiPostTable(TikTokDTO content)
        {
            int res = 0;

            try
            {
                await _conn.OpenAsync();

                //string query = "insert ignore into social_index_v2.tiktok_source_post " +
                //    "(post_id,si_demand_source_id,platform,link,create_time,update_time,crawled_time,status,total_comment) " +
                //    "values (@post_id,@platform,@link,@create_time,@update_time,@crawled_time,@status,@total_comment)";
                //MySqlCommand cmd = new MySqlCommand();
                //cmd.Connection = _conn;
                //cmd.CommandText = query;
                //cmd.Parameters.AddWithValue("@post_id", content.post_id);
                //cmd.Parameters.AddWithValue("@platform", content.platform);
                //cmd.Parameters.AddWithValue("@link", content.link);
                //cmd.Parameters.AddWithValue("@si_demand_source_id", content.si_demand_source_id);
                //cmd.Parameters.AddWithValue("@create_time", content.create_time);
                //cmd.Parameters.AddWithValue("@update_time", content.update_time);
                //cmd.Parameters.AddWithValue("@crawled_time", content.crawled_time);
                //cmd.Parameters.AddWithValue("@status", content.status);
                //cmd.Parameters.AddWithValue("@total_comment", content.total_comment);
                string query = @"insert ignore into si_demand_source_post(si_demand_source_id, post_id, platform, link, create_time, update_time, crawled_time, status,  total_comment, user_crawler, index_slave) 
                                values (@si_demand_source_id, @post_id, @platform, @link, @create_time, @update_time, @crawled_time, @status,  @total_comment, @user_crawler, @index_slave);";
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@si_demand_source_id", content.si_demand_source_id);
                cmd.Parameters.AddWithValue("@post_id", content.post_id);
                cmd.Parameters.AddWithValue("@platform", content.platform);
                cmd.Parameters.AddWithValue("@link", content.link);
                cmd.Parameters.AddWithValue("@create_time", content.create_time);
                cmd.Parameters.AddWithValue("@update_time", content.update_time);
                cmd.Parameters.AddWithValue("@crawled_time", content.crawled_time);
                cmd.Parameters.AddWithValue("@status", content.status);
                cmd.Parameters.AddWithValue("@total_comment", content.total_comment);
                cmd.Parameters.AddWithValue("@user_crawler", content.user_crawler);
                cmd.Parameters.AddWithValue("@index_slave", content.index_slave);



                await cmd.ExecuteNonQueryAsync();

                res = 1;

            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                if (ex.Message.ToLower().Contains("duplicate entry"))
                {
                    res = -2;
                }
                else
                {
                    res = -1;
                }
            }

            return res;
        }
        /// <summary>
        /// Insert Content to tiktok_link table (bảng local để lưu Link)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<int> InserTikTokLinkTable(TikTokDTO content)
        {
            int res = 0;

            try
            {
                await _conn.OpenAsync();

                string query = "insert ignore example.tiktok_link " +
                    "(post_id,link,domain,status) " +
                    "values (@post_id,@link,@domain,@status)";

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("@post_id", content.post_id);
                cmd.Parameters.AddWithValue("@link", content.link);
                cmd.Parameters.AddWithValue("@domain", content.domain);
                cmd.Parameters.AddWithValue("@status", content.status_link);

                await cmd.ExecuteNonQueryAsync();

                res = 1;

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate entry"))
                {
                    res = -2; // trùng link
                }
                else
                {
                    res = -1; // lỗi, bắt lỗi trả ra để sửa

                    // ghi lỗi xuống fil
                }
            }

            return res;
        }

        /// <summary>
        /// Insert Content to tiktok_source_post table (bảng local để test)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<int> InserTikTokSourcePostTable(TikTokDTO content)
        {
            int res = 0;
            try
            {
                await _conn.OpenAsync();
                string query = @"insert ignore into si_demand_source_post(si_demand_source_id, post_id, platform, link, create_time, update_time, crawled_time, status,  total_comment, user_crawler, index_slave) 
                                values (@si_demand_source_id, @post_id, @platform, @link, @create_time, @update_time, @crawled_time, @status,  @total_comment, @user_crawler, @index_slave);";
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@si_demand_source_id", content.si_demand_source_id);
                cmd.Parameters.AddWithValue("@post_id", content.post_id);
                cmd.Parameters.AddWithValue("@platform", content.platform);
                cmd.Parameters.AddWithValue("@link", content.link);
                cmd.Parameters.AddWithValue("@create_time", content.create_time);
                cmd.Parameters.AddWithValue("@update_time", content.update_time);
                cmd.Parameters.AddWithValue("@crawled_time", content.crawled_time);
                cmd.Parameters.AddWithValue("@status", content.status);
                cmd.Parameters.AddWithValue("@total_comment", content.total_comment);
                cmd.Parameters.AddWithValue("@user_crawler", content.user_crawler);
                cmd.Parameters.AddWithValue("@index_slave", content.index_slave);
                await cmd.ExecuteNonQueryAsync();

                res = 1;

                _conn.Close();
                Logging.Infomation("Thêm thành công");

            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                if (ex.Message.ToLower().Contains("duplicate entry"))
                {
                    res = -2; // trùng link
                    Logging.Error(ex, "Trùng link");
                }
                else
                {
                    res = -1; // lỗi, bắt lỗi trả ra để sửa
                    // ghi lỗi xuống fil
                }
            }
            return res;
        }

        /// <summary>
        /// Select URL from tiktok_link table để bóc
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<TikTokDTO> GetLinkByDomain(string domain)
        {
            List<TikTokDTO> data = new List<TikTokDTO>();
            string query = $"Select * from example.tiktok_link where domain ='{domain}'";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, _conn))
                {
                    _conn.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new TikTokDTO
                            {
                                link = reader["link"].ToString(),
                                status_link = (int)reader["status"],
                            }
                            );
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _conn.Close();

            return data;
        }
        public async Task TruncateLink_Db()
        {
            await _conn.OpenAsync();
            string query = "TRUNCATE TABLE example.tiktok_link";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _conn.Close();

        }

        /// <summary>
        /// Update status
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<int> UpdateStatus(string link)
        {
            TikTokDTO content = new TikTokDTO();
            int res = 0;
            try
            {
                await _conn.OpenAsync();

                string query = $"UPDATE example.tiktok_link SET status = 1 WHERE link = '{link}'";

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@status", content.status_link);
                await cmd.ExecuteNonQueryAsync();

                res = 1;


            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate entry"))
                {
                    res = -2; // trùng link
                }
                else
                {
                    res = -1; // lỗi, bắt lỗi trả ra để sửa

                    // ghi lỗi xuống fil
                }
            }
            return res;
        }

        /// <summary>
        /// Select hashtag from si_hashtag table between from date to date
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<string> GetHashtagInTableSiHastag(string fromDate, string toDate)
        {
            List<string> data = new List<string>();
            string query = $"Select * from social_index_v2.si_hashtag WHERE create_time BETWEEN '{fromDate}' AND '{toDate}' ";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, _conn))
                {
                    _conn.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(reader["hashtag"].ToString());

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _conn.Close();

            return data;
        }

        /// <summary>
        /// Select hashtag from si_hashtag table by current date
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<List<string>> GetHashtagInTableSiHastagByCurrentDate(string datetime)
        {
            List<string> data = new List<string>();
            //string query = $"select * from social_index_v2.tiktok_hastag  WHERE create_time = '{datetime}' ";
            string query = $"select * from social_index_v2.si_hashtag ";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, _conn))
                {
                    _conn.Open();
                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            data.Add(reader["hashtag"].ToString());

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            _conn.Close();

            return data;
        }

    }
}
