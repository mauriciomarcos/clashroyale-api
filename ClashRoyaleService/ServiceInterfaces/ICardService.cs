using ClashRoyaleDomain;
using System.Collections.Generic;

namespace ClashRoyaleService.ServiceInterfaces
{
    public interface ICardService
    {
        IEnumerable<Card> GetAllCards();
    }
}
