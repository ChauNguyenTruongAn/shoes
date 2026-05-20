using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Payments
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}