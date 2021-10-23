using System.Threading.Tasks;

namespace FoxyMonitor.Contracts.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle();

        Task HandleAsync();
    }
}
