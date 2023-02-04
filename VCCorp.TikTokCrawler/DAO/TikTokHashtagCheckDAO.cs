using Crwal.Core.Base;
using Crwal.Core.Log;
using Crwal.Core.Sql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCCorp.TikTokCrawler.Model;

namespace VCCorp.TikTokCrawler.DAO
{
    public class TikTokHashtagCheckDAO
    {
        private readonly MySqlConnection _conn = new MySqlConnection(TiktokRuntime.Config.DbConnection.ConnectionToTableSiPost);
        private readonly string _connStr = TiktokRuntime.Config.DbConnection.ConnectionToTableSiPost;
        //public TikTokHashtagCheckDAO(string connStr)
        //{
        //    _conn = new MySqlConnection(connStr);
        //}
        public async Task Insert(TikTokHastagCheckDTO model)
        {

            Logging.Infomation("Insert dữ liệu tiktok_hashtag_check");
            string query = model.SqlInsert();
            try
            {
                Logging.Infomation("Bắt đầu thêm dữ liệu");
                await _conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@hashtag", model.hashtag);
                cmd.Parameters.AddWithValue("@url_ids", model.url_ids);
                cmd.Parameters.AddWithValue("@created_at", model.created_at);
                cmd.Parameters.AddWithValue("@updated_at", model.updated_at);
                await cmd.ExecuteNonQueryAsync();

                //Logging.Infomation(model.ToJson());
                Logging.Infomation("Thêm dữ liệu thành công");
            }
            catch (Exception ex)
            {

                Logging.Error(ex, "TikTokHashtagCheckDAO - Insert");
            }
            await _conn.CloseAsync();
        }
        public async Task<List<TikTokHastagCheckDTO>> SelectAllAsync()
        {
            Logging.Infomation("Đọc dữ liệu tiktok_hashtag_check");
            List<TikTokHastagCheckDTO> datas = new List<TikTokHastagCheckDTO>();
            string query = "SELECT hashtag,url_ids,created_at,updated_at FROM tiktok_hashtag_check";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, _conn))
                {
                    if (_conn.State == ConnectionState.Closed)
                    {
                        await _conn.OpenAsync();
                    }
                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            datas.Add(new TikTokHastagCheckDTO()
                            {
                                hashtag = reader["hashtag"].ToString(),
                                created_at = DateTime.Parse(reader["created_at"].ToString()),
                                updated_at = DateTime.Parse(reader["updated_at"].ToString()),
                                url_ids = reader["url_ids"].ToString()
                            });

                        }
                    }
                }
                await _conn.CloseAsync();
                return datas;
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "TikTokHashtagCheckDAO - SelectAll");
            }
            return null;
        }
        public async Task<TikTokHastagCheckDTO> SelectByHashtagAsync(string hashtag)
        {
            Logging.Infomation("Đọc dữ liệu tiktok_hashtag_check - " + hashtag);
            List<TikTokHastagCheckDTO> datas = new List<TikTokHastagCheckDTO>();
            string query = $"SELECT hashtag,url_ids,created_at,updated_at FROM tiktok_hashtag_check where hashtag = '{hashtag}'";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, _conn))
                {
                    if (_conn.State == ConnectionState.Closed)
                    {
                        await _conn.OpenAsync();
                    }
                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            datas.Add(new TikTokHastagCheckDTO()
                            {
                                hashtag = reader["hashtag"].ToString(),
                                created_at = DateTime.Parse(reader["created_at"].ToString()),
                                updated_at = DateTime.Parse(reader["updated_at"].ToString()),
                                url_ids = reader["url_ids"].ToString()
                            });

                        }
                    }

                }
                await _conn.CloseAsync();
                if (datas != null && datas.Count > 0)
                {
                    return datas[0];
                }
                return new TikTokHastagCheckDTO() { };
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "TikTokHashtagCheckDAO - SelectByHashtagAsync");
            }
            return null;
        }
        public async Task UpdateAsync(TikTokHastagCheckDTO videoIds)
        {
            string query = $"update tiktok_hashtag_check set url_ids = @url_ids, updated_at = @updated_at where hashtag = '{videoIds.hashtag}'";
            Logging.Infomation("Update dữ liệu tiktok_hashtag_check");
            try
            {
                if (_conn.State == ConnectionState.Closed)
                {
                    await _conn.OpenAsync();
                }
                Logging.Infomation("Bắt đầu update dữ liệu");
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@url_ids", videoIds.url_ids);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                await cmd.ExecuteNonQueryAsync();

                //Logging.Infomation(videoIds.ToJson());
                Logging.Infomation("Update dữ liệu thành công");
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "TikTokHashtagCheckDAO - UpdateAsync");
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<string> GetLastTag()
        {
            string lastTag = "";
            string query = $"SELECT hashtag,url_ids,created_at,updated_at FROM tiktok_hashtag_check ORDER BY updated_at DESC LIMIT 1";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, _conn))
                {
                    if (_conn.State == ConnectionState.Closed)
                    {
                        await _conn.OpenAsync();
                    }
                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            lastTag = reader["hashtag"].ToString();
                        }
                    }
                }
                return lastTag;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return lastTag;
        }
    }
}
