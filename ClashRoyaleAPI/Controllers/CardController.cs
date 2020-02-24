using ClashRoyaleDomain;
using ClashRoyaleService.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace ClashRoyaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly IMemoryCache _memoryCache;

        public CardController(ICardService cardService, IMemoryCache memoryCache)
        {
            _cardService = cardService;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Card), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Card>> Get()
        {
            var memoryCache = _memoryCache.GetOrCreate("allCards", cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(10);
                return Ok(_cardService.GetAllCards());
            });

            
            return Ok(memoryCache);
        }
    }
}