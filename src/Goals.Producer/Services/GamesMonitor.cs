using Goals.Producer.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Goals.Producer.Services
{
    public class GamesMonitor : IHostedService, IDisposable
    {
        readonly ILogger<GamesMonitor> _logger;

        readonly FootballDataSource _footballDataSource;

        readonly FootballDataTarget _footballDataTarget;

        CancellationToken _cancellationToken;

        Timer _timer;


        public GamesMonitor(ILogger<GamesMonitor> logger, FootballDataSource footballDataSource, FootballDataTarget footballDataTarget) =>
            (_logger, _footballDataSource, _footballDataTarget) = (logger, footballDataSource, footballDataTarget)
        ;


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _timer = new Timer(CheckGames, null, TimeSpan.Zero, TimeSpan.FromSeconds(_footballDataSource.RefreshIntervalInSeconds));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();


        private async void CheckGames(object state)
        {
            await foreach(var match in _footballDataSource.GetMatchUpdates())
                await _footballDataTarget.PersistMatch(match, _cancellationToken);
        }
    }
}
