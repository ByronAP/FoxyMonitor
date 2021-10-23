using System;

namespace FoxyMonitor.Contracts.Services
{
    public interface IApplicationInfoService
    {
        Version GetVersion();
    }
}
