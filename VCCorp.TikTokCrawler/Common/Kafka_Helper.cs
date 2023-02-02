using Confluent.Kafka;
using Crwal.Core.Log;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using VCCorp.TikTokCrawler.Model;

namespace VCCorp.TikTokCrawler.Common
{
    class Kafka_Helper
    {
        private static string SERVER_LINK = TiktokRuntime.Config.ConfigSystem.KafkaConfig.SeverLink;

        public static string _topicPost = TiktokRuntime.Config.ConfigSystem.KafkaConfig.TopicTikTokPost;

        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = SERVER_LINK,
            Partitioner = Partitioner.ConsistentRandom,
            ClientId = Dns.GetHostName()
        };

        private static ProducerConfig _config = new ProducerConfig
        {
            BootstrapServers = SERVER_LINK,
            ClientId = Dns.GetHostName(),
            Partitioner = Partitioner.Random
        };

        private static IProducer<string, string> producer = new ProducerBuilder<string, string>(_config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

        public async Task<bool> InsertPost(string messagejson, string topic)
        {
            Logging.Infomation("Bắt đầu gửi dữ liệu vào kafka");
            try
            {
                DeliveryResult<string, string> val = await producer.ProduceAsync(topic, new Message<string, string> { Value = messagejson });
                producer.Flush(TimeSpan.FromMilliseconds(100));
                Logging.Infomation($"Dữ liệu bắn vào kafka: messagejson: {messagejson}, topic: {topic}");
                return true;
            }
            catch (Exception ex)
            {
                File.AppendAllText($"{Environment.CurrentDirectory}/Check/kafka.txt", ex.ToString() + "\n");
                Logging.Error(ex, $"Quá trình bắn dữ liệu vào kafka bị lỗi");

            }
            return false;
        }
        public static string ToJson<T>(T obj)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize<T>(obj);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
