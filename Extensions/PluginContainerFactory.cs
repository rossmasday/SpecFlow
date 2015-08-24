using System.ComponentModel.Composition;
using TechTalk.SpecFlow.BoDi;

namespace TechTalk.Specflow.Extensions
{
    [Export(typeof (IPluginContainerFactory))]
    public class PluginContainerFactory : IPluginContainerFactory
    {
        public IObjectContainer CreateContainer()
        {
            return new NinjectPlugin.SpecFlowStandardKernel();
        }

        public IObjectContainer CreateContainer(IObjectContainer container)
        {
            return new NinjectPlugin.SpecFlowStandardKernel(container);
        }
    }
}