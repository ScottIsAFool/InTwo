using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ScoreoidPortable;
using ScoreoidPortable.Entities;

namespace InTwo.Design
{
    public class DesignScoreoidClient : IScoreoidClient
    {
        public Task<bool> SignInAsync(string username, string password = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPlayerCountAsync(DateTime? startDate = null, DateTime? endDate = null, string platform = null, int difficulty = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Player> GetPlayerAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePlayerAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreatePlayerAsync(Player player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditPlayerAsync(Player player)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPlayerRankAsync(string username, DateTime? startDate = null, DateTime? endDate = null, string platform = null, int difficulty = 0)
        {
            throw new NotImplementedException();
        }

        public Task<List<Score>> GetPlayerScoresAsync(string username, SortBy? sortBy = null, OrderBy? orderBy = null, int? startingAt = null, int? numberToRetrieve = null, DateTime? startDate = null, DateTime? endDate = null, string platform = null, int difficulty = 0)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateScoreAsync(string username, Score score)
        {
            throw new NotImplementedException();
        }

        public Task<double> CountScoresAsync(DateTime? startDate = null, DateTime? endDate = null, string platform = null, int difficulty = 0)
        {
            throw new NotImplementedException();
        }

        public Task<double> CountBestScoresAsync(DateTime? startDate = null, DateTime? endDate = null, string platform = null, int difficulty = 0)
        {
            throw new NotImplementedException();
        }

        public Task<List<ScoreItem>> GetScoresAsync(SortBy? sortBy = null, OrderBy? orderBy = null, int? startingAt = null, int? numberToRetrieve = null, DateTime? startDate = null, DateTime? endDate = null, string platform = null, int difficulty = 0, string usernames = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<ScoreItem>> GetBestScoresAsync(SortBy? sortBy = null, OrderBy? orderBy = null, int? startingAt = null, int? numberToRetrieve = null, DateTime? startDate = null, DateTime? endDate = null, string platform = null, int difficulty = 0, string usernames = null)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetAverageScoreAsync(DateTime? startDate = null, DateTime? endDate = null, string platform = null, int difficulty = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Game> GetGameAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Player>> GetGamePlayersAsync(SortBy? sortBy = null, OrderBy? orderBy = null, int? startingAt = null, int? numberToRetrieve = null, DateTime? startDate = null, DateTime? endDate = null, string platform = null)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetGameAverageAsync(GameField gameField, DateTime? startDate = null, DateTime? endDate = null, string platform = null)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetGameTopAsync(GameField gameField, DateTime? startDate = null, DateTime? endDate = null, string platform = null)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetGameLowestAsync(GameField gameField, DateTime? startDate = null, DateTime? endDate = null, string platform = null)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetGameTotalAsync(GameField gameField, DateTime? startDate = null, DateTime? endDate = null, string platform = null)
        {
            throw new NotImplementedException();
        }

        public HttpClient HttpClient
        {
            get;
            private set;
        }

        public string ApiKey
        {
            get;
            set;
        }

        public string GameId
        {
            get;
            set;
        }
    }
}