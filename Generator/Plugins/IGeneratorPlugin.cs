using BoDi;
using TechTalk.SpecFlow.BoDi;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPlugin
    {
        void RegisterDependencies(IObjectContainer container);
        void RegisterCustomizations(IObjectContainer container, SpecFlowProjectConfiguration generatorConfiguration);
        void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration);
    }
}