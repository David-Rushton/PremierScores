using Confluent.Kafka;
using Goals.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace Goals.Consumer
{
    public static class GoalsConsumer
    {
        static readonly string _consumerGroupId = "Goals.Consumer.000";


        [FunctionName("GoalsConsumer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var envVariables = GetEnvironmentVariables();
            var config = GetConfig(envVariables.bootstrapServers, envVariables.username, envVariables.password);
            var footballMatches = new List<FootballMatch>();
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var args = GetArgs();
            using var consumer = new ConsumerBuilder<string, string>(config).Build();


            // Ensure require args are provided.
            if(args.validationErrorMessage != string.Empty)
                return new BadRequestObjectResult(args.validationErrorMessage);


            log.LogInformation($"Reading from topic: {envVariables.topic}");
            consumer.Subscribe(envVariables.topic);

            try
            {
                while(true)
                {
                    // This is ugly.
                    // We read messages until the topic throws an exception.
                    // At that point we _assume_ we have reached the end of the queue.
                    // Next time the consumer should persist to an intermidate store.
                    var item = consumer.Consume(new TimeSpan(0, 0, 3));

                    log.LogInformation($"Read item from topic: {item.Message.Key}");
                    if(IsFootballMatchMessage(item.Message.Key, item.Message.Value))
                    {
                        log.LogInformation($"Item is a match: {item.Message.Value}");
                        var match = JsonSerializer.Deserialize<FootballMatch>(item.Message.Value, jsonOptions);
                        footballMatches.Add(match);
                    }
                }
            }
            catch(NullReferenceException)
            {
                log.LogInformation($"Topic { envVariables.topic } exhausted.");
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult(string.Format("{ \"error\": \"{0}\" }", e.Message));
            }


            consumer.Close();
            log.LogInformation("Topic empty");
            return new OkObjectResult($"here it is:\n\n");


            (string bootstrapServers, string topic, string username, string password) GetEnvironmentVariables() =>
            (
                Environment.GetEnvironmentVariable("KAFKA_BOOTSTAP_SERVERS"),
                Environment.GetEnvironmentVariable("KAFKA_TOPIC"),
                Environment.GetEnvironmentVariable("KAFKA_USERNAME"),
                Environment.GetEnvironmentVariable("KAFKA_PASSWORD")
            );

            ConsumerConfig GetConfig(string bootstrapServers, string username, string password) =>
                new ConsumerConfig
                {
                    BootstrapServers = bootstrapServers,
                    GroupId = _consumerGroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = username,
                    SaslPassword = password
                }
            ;

            (string validationErrorMessage, DateTime dateFrom, DateTime dateUntil) GetArgs()
            {
                var dateFrom = DateTime.MinValue;
                var dateUntil = DateTime.MinValue;

                foreach(var dateArg in new [] {"dateFrom", "dateUntil"})
                    if(req.Query.ContainsKey(dateArg))
                        if(DateTime.TryParse(req.Query[dateArg], out var dateTimeResult))
                            if(dateArg == "dateFrom")
                                dateFrom = dateTimeResult;
                            else
                                dateUntil = dateTimeResult;

                // Why do this here?
                // Why do this like this?
                var validationErrorMessage = string.Empty;
                validationErrorMessage += dateFrom != DateTime.MinValue ? "Required parameter dateFrom not supplied.  " : "";
                validationErrorMessage += dateUntil != DateTime.MinValue ? "Required parameter dateUntil not supplied.  " : "";
                validationErrorMessage += dateFrom >= dateUntil ? "dateUtil must occur after dateFrom.  " : "";

                return (validationErrorMessage, dateFrom, dateUntil);
            }

            bool IsFootballMatchMessage(string key, string value)
            {
                if(value.ToLower().Contains("id") && value.ToLower().Contains("lastupdated"))
                    return true;

                return false;
            }










        }
    }
}
