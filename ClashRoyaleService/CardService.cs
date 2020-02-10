using System.Collections.Generic;
using ClashRoyaleDomain;
using ClashRoyaleService.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ClashRoyaleService
{
    public class CardService : ICardService
    {
        private readonly string _appSettingsAPIKey;

        public CardService(IConfiguration configuration)
        {
            _appSettingsAPIKey = configuration.GetSection("Api-Key").Value;
        }

        public IEnumerable<Card> GetAllCards()
        {
            var apiKey = _appSettingsAPIKey;
            var client = new RestClient("https://api.clashroyale.com/v1/cards");
            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", apiKey);
            IRestResponse<IEnumerable<Card>> response = client.Execute<IEnumerable<Card>>(request);

            var json = JObject.Parse(response.Content);

            return FillCardList(json);
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