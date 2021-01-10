using Confluent.Kafka;
using Goals.Producer.Config;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;


namespace Goals.Producer.Repositories
{
    public class FootballDataTarget
    {
        readonly ILogger<FootballDataTarget> _logger;

        readonly string _topicName;

        readonly IProducer<int, string> _producer;


        public FootballDataTarget(ILogger<FootballDataTarget> logger, KafkaConfig kafkaConfig)
        {
             var config = new ProducerConfig
             {
                 BootstrapServers = kafkaConfig.BootstrapServers,
                 SecurityProtocol = SecurityProtocol.SaslSsl,
                 SaslMechanism = SaslMechanism.Plain,
                 SaslUsername = kafkaConfig.Username,
                 SaslPassword = kafkaConfig.Password
             };


            _topicName = kafkaConfig.Topic;
            _producer = new ProducerBuilder<int, string>(config).Build();
            _logger = logger;
        }


        public async Task PersistMatch(FootballMatch match, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Persisting match: {match.Id}");
                await _producer.ProduceAsync
                (
                    _topicName,
                    new Message<int, string>
                    {
                        Key = match.Id, Value = JsonSerializer.Serialize(match)
                    },
                    cancellationToken
                );
            }
            catch (ProduceException<int, string> e)
            {
                _logger.LogError($"Cannot persist football match: {match.Id}.  Because: {e.Message}.");
            }
        }
    }
}
