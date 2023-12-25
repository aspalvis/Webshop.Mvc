using Braintree;

namespace Utility.BrainTree
{
    public interface IBrainTreeGate
    {
        IBraintreeGateway CreateGateWay();
        IBraintreeGateway GetGateWay();
    }
}
