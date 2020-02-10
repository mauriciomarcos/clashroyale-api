using ClashRoyaleDomain;
using ClashRoyaleService.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ClashRoyaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Card), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Card>> Get()
        {
            var cards = _cardService.GetAllCards();
            return Ok(cards);
        }
    }
}