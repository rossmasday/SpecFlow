using BoDi;
using TechTalk.Specflow.Extensions;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IDefaultDependencyProvider
    {
        void RegisterDefaults(IObjectContainer container);
    }
}