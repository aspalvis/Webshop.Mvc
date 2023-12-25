using Braintree;
using Microsoft.Extensions.Options;

namespace Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        private IBraintreeGateway _gateway;

        public BrainTreeSettings Options { get; set; }

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            Options = options.Value;
        }

        public IBraintreeGateway CreateGateWay() => new BraintreeGateway(Options.Environment, Options.MerchantId, Options.PublicKey, Options.PrivateKey);
        public IBraintreeGateway GetGateWay()
        {
            _gateway ??= CreateGateWay();

            return _gateway;
        }
    }
}
