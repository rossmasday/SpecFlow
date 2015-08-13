using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IDefaultDependencyProvider
    {
        void RegisterDefaults(IObjectContainer container);
    }
}