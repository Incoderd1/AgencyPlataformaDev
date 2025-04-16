using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntent(decimal amount, string currency, string description, Dictionary<string, string> metadata);
        Task<bool> ConfirmPayment(string paymentIntentId);
        string CreateCheckoutSession(decimal amount, string currency, string productName, string successUrl, string cancelUrl);
        string ValidateWebhook(string json, string signatureHeader);
    }
}
