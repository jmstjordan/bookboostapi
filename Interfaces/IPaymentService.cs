
namespace BookBoostApi.Interfaces;

public interface IPaymentService
{
    public Task<string> CreatePayment(PaymentRequest request);
}

