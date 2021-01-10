namespace Goals.Producer.Config
{
    public class KafkaConfig
    {
        public string BootstrapServers { get; init; }

        public string Topic { get; init; }

        public string Username { get; init; }

        public string Password { get; init; }
    }
}
