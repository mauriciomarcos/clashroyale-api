using ClashRoyaleDomain;
using ClashRoyaleService.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

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

        /// <summary> Construtor que recebe parâmentros dos tipo ICardService e IMemoryCache por injeção de dependência.</summary>
        /// <param name="cardService"></param>
        /// <param name="memoryCache"></param>
        public CardController(ICardService cardService, IMemoryCache memoryCache)
        {
            _cardService = cardService;
            _memoryCache = memoryCache;
        }

        /// <summary>Método Get() obtém todos os Cards do jogo.</summary>
        /// <response code="200">Quando o método Get() consiguir se comunicar com a API do Clash Royale e retornar a lista com todos os Cards do jogo.</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Card>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Card>> Get()
        {
            var memoryCache = _memoryCache.GetOrCreate("allCards", cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(10);
                return Ok(_cardService.GetAllCards());
            });
            
            return Ok(memoryCache);
        }

        /// <summary>Retorna o Card a partir do id enviado como parâmetro.</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:long}")]
        [ProducesResponseType(typeof(Card), StatusCodes.Status200OK)]
        public ActionResult<Card> Get(long id)
        {
            var card = _cardService.GetById(id);
            return Ok(card);
        }
    }
}