using System.Collections.Generic;
using System.Linq;
using ClashRoyaleDomain;
using ClashRoyaleService.ServiceInterfaces;
using ClashRoyaleUtils.Configurations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ClashRoyaleService
{
    public class CardService : ICardService
    {
        private readonly ConfigurationAPI _apiConfiguration;

        public CardService(IOptions<ConfigurationAPI> apiOptions)
        {
            _apiConfiguration = apiOptions.Value;
        }

        public IEnumerable<Card> GetAllCards()
        {
            var client = new RestClient(_apiConfiguration.UriGetCards);
            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", string.Format("{0}{1}", "Bearer ", _apiConfiguration.ApiKey));
            IRestResponse<IEnumerable<Card>> response = client.Execute<IEnumerable<Card>>(request);

            var json = JObject.Parse(response.Content);

            return FillCardList(json);
        }

        public Card GetById(long id)
        {
            var client = new RestClient(_apiConfiguration.UriGetCards);
            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", string.Format("{0}{1}", "Bearer ", _apiConfiguration.ApiKey));
            IRestResponse<IEnumerable<Card>> response = client.Execute<IEnumerable<Card>>(request);

            var json = JObject.Parse(response.Content);

            return FillCardList(json)
                .ToList()
                .Where(card => card.Id == id)
                .FirstOrDefault();
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