using System;
using System.Collections.Generic;
using System.Linq;
using ClashRoyaleDomain;
using ClashRoyaleService.ServiceInterfaces;
using ClashRoyaleUtils.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ClashRoyaleService
{
    public class CardService : ICardService
    {
        private readonly ConfigurationAPI _apiConfiguration;
        private readonly AuthConfig _authConfig;
        private readonly ILogger<CardService> _logger;

        public CardService(
            IOptions<ConfigurationAPI> apiOptions, 
            IOptions<AuthConfig> authConfig, 
            ILogger<CardService> logger
            )
        {
            _apiConfiguration = apiOptions.Value;
            _authConfig = authConfig.Value;
            _logger = logger;
        }

        public IEnumerable<Card> GetAllCards()
        {
            _logger.Log(LogLevel.Information, $"Início da chamada do método GetAllCards - CardService {DateTime.Now}");

            var client = new RestClient(_apiConfiguration.UriGetCards);
            var request = new RestRequest(Method.GET);

            try
            {
                request.AddHeader("Authorization", string.Format("{0}{1}", "Bearer ", _apiConfiguration.ApiKey));

                _logger.Log(LogLevel.Information, $"Início da chamada ao endpoint do Clash Royale - CardService {DateTime.Now}");
                IRestResponse<IEnumerable<Card>> response = client.Execute<IEnumerable<Card>>(request);
                _logger.Log(LogLevel.Information, $"Fim da chamada ao endpoint do Clash Royale - CardService {DateTime.Now}");

                var json = JObject.Parse(response.Content);

                return FillCardList(json);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.StackTrace, $"{ex.Message} - {DateTime.Now}");
                throw ex;
            }
         
        }

        public Card GetById(long id)
        {
            var client = new RestClient(_apiConfiguration.UriGetCards);
            var request = new RestRequest(Method.GET);

            try
            {
                request.AddHeader("Authorization", string.Format("{0}{1}", "Bearer ", _apiConfiguration.ApiKey));
                IRestResponse<IEnumerable<Card>> response = client.Execute<IEnumerable<Card>>(request);

                var json = JObject.Parse(response.Content);

                return FillCardList(json)
                    .ToList()
                    .Where(card => card.Id == id)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        #region | MÉTODOS AUXILIARES |
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private IEnumerable<Card> FillCardList(JObject json)
        {
            var cards = new List<Card>();
            JToken jcards = json["items"];

            foreach (var item in jcards)
            {
                Card card = new Card()
                {
                    Id = (int)item["id"],
                    Name = (string)item["name"],
                    MaxLevel = (int)item["maxLevel"],
                    IconUrls = new IconUrl()
                    {
                        Medium = (string)item["iconUrls"].ToObject<JToken>()["medium"]
                    }
                };

                cards.Add(card);
            }

            return (IEnumerable<Card>)cards;
        }
        #endregion
    }
}