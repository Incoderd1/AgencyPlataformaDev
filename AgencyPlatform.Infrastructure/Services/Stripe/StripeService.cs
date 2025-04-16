using AgencyPlatform.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Infrastructure.Services.Stripe
{
    public class StripeService : IPaymentService
    {
        private readonly string _apiKey;
        private readonly string _webhookSecret;

        public StripeService(IConfiguration configuration)
        {
            _apiKey = configuration["Stripe:SecretKey"];
            _webhookSecret = configuration["Stripe:WebhookSecret"];
            StripeConfiguration.ApiKey = _apiKey;
        }

        public async Task<string> CreatePaymentIntent(decimal amount, string currency, string description, Dictionary<string, string> metadata)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe usa centavos
                Currency = currency.ToLower(),
                Description = description,
                Metadata = metadata,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return intent.ClientSecret;
        }

        public async Task<bool> ConfirmPayment(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var intent = await service.GetAsync(paymentIntentId);

                return intent.Status == "succeeded";
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string CreateCheckoutSession(decimal amount, string currency, string productName, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(amount * 100),
                            Currency = currency.ToLower(),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = productName,
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            var session = service.Create(options);

            return session.Url;
        }

        public string ValidateWebhook(string json, string signatureHeader)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signatureHeader,
                    _webhookSecret
                );

                return stripeEvent.Type;
            }
            catch (StripeException)
            {
                return null;
            }
        }
    }
}
