using ClashRoyaleDomain;
using ClashRoyaleService.ServiceInterfaces;
using ClashRoyaleUtils.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClashRoyaleAPI.Controllers
{
    /// <summary>
    /// Controller que possibilita a manutenção e obtenção dos dados referentes aos Cards do Clash Royale.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        /// <summary> Construtor que recebe parâmentros dos tipo ICardService e IMemoryCache por injeção de dependência.</summary>
        /// <param name="cardService">ICardService</param>
        /// <param name="memoryCache">IMemoryCache</param>
        /// <param name="distributedCache">IDistributedCache</param>
        public CardController(ICardService cardService, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _cardService = cardService;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        /// <summary>Método Get() obtém todos os Cards do jogo.</summary>
        /// <response code="200">Quando o método Get() consiguir se comunicar com a API do Clash Royale e retornar a lista com todos os Cards do jogo.</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Card>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Card>> Get()
        {
            IEnumerable<Card> cards = null;

            /*
           * Criada a classe DistributedCacheExtension em Utils.Extension para exentender o método Get da interface IDistributedCache.
           * Objetivo é facilitar a criação de um cache distribuído reapreveitando código.
           */
            cards = _distributedCache.Get<IEnumerable<Card>>("GetAllCards");

            if (cards != null)
                return Ok(cards);
           
            cards = _cardService.GetAllCards();

            /*
             * Criada a classe DistributedCacheExtension em Utils.Extension para exentender o método Set da interface IDistributedCache.
             * Objetivo é facilitar a criação de um cache distribuído reapreveitando código.
             */
            _distributedCache.Set<IEnumerable<Card>>("GetAllCards", cards, TimeSpan.FromSeconds(30));

            return Ok(cards);
        }

        /// <summary>Retorna o Card a partir do id enviado como parâmetro.</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:long}")]
        [ProducesResponseType(typeof(Card), StatusCodes.Status200OK)]
        public ActionResult<Card> Get(long id)
        {
            var memoryCache = _memoryCache.GetOrCreate($"CardId:{id}", cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(10);
                return Ok(_cardService.GetById(id));
            });

            return Ok(memoryCache);
        }
    }
}