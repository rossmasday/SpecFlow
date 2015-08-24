using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BoDi;
using System.Linq;
using TechTalk.SpecFlow.BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestRunContainerBuilder
    {
        IObjectContainer CreateContainer(IRuntimeConfigurationProvider configurationProvider = null);
    }

    public class TestRunContainerBuilder : ITestRunContainerBuilder
    {
        [Import(typeof (IPluginContainerFactory), AllowDefault = true)]
        private IPluginContainerFactory objectContainerFactory;

        public static IDefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        private readonly IDefaultDependencyProvider defaultDependencyProvider;

        public TestRunContainerBuilder(IDefaultDependencyProvider defaultDependencyProvider = null)
        {
            this.defaultDependencyProvider = defaultDependencyProvider ?? DefaultDependencyProvider;
        }

        public virtual IObjectContainer CreateContainer(IRuntimeConfigurationProvider configurationProvider = null)
        {
            var composer = new Composer();
            composer.Compose(this);

            var container = objectContainerFactory == null
                ? new ObjectContainer()
                : objectContainerFactory.CreateContainer();

            RegisterDefaults(container);

            if (configurationProvider != null)
                container.RegisterInstanceAs(configurationProvider);

            configurationProvider = configurationProvider ?? container.Resolve<IRuntimeConfigurationProvider>();

            var plugins = LoadPlugins(configurationProvider, container);
            foreach (var plugin in plugins)
                plugin.RegisterDependencies(container);

            RuntimeConfiguration runtimeConfiguration = new RuntimeConfiguration();

            foreach (var plugin in plugins)
                plugin.RegisterConfigurationDefaults(runtimeConfiguration);

            configurationProvider.LoadConfiguration(runtimeConfiguration);

#if !BODI_LIMITEDRUNTIME
            if (runtimeConfiguration.CustomDependencies != null)
            {
                var registerConfiguration = new RegisterConfiguration(container);
                registerConfiguration.RegisterFromConfiguration(runtimeConfiguration.CustomDependencies);
            }              
#endif

            container.RegisterInstanceAs(runtimeConfiguration);

            if (runtimeConfiguration.RuntimeUnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestRuntimeProvider>(runtimeConfiguration.RuntimeUnitTestProvider));

            foreach (var plugin in plugins)
                plugin.RegisterCustomizations(container, runtimeConfiguration);

            return container;
        }

        protected virtual IRuntimePlugin[] LoadPlugins(IRuntimeConfigurationProvider configurationProvider, IObjectContainer container)
        {
            //TODO RA: at no point in any of the tests does this return any more than an empty dictionary therefore i am creating this Default dependencyProvider
            var plugins = container.Resolve<IDictionary<string, IRuntimePlugin>>().Values.AsEnumerable();

            var pluginLoader = container.Resolve<IRuntimePluginLoader>();
            plugins = plugins.Concat(configurationProvider.GetPlugins().Where(pd => (pd.Type & PluginType.Runtime) != 0).Select(pd => LoadPlugin(pluginLoader, pd)));

            return plugins.ToArray();
        }

        protected virtual IRuntimePlugin LoadPlugin(IRuntimePluginLoader pluginLoader, PluginDescriptor pluginDescriptor)
        {
            return pluginLoader.LoadPlugin(pluginDescriptor);
        }

        protected virtual void RegisterDefaults(IObjectContainer container)
        {
            defaultDependencyProvider.RegisterDefaults(container);
        }

        // used by tests
        internal static IObjectContainer CreateDefaultContainer(IRuntimeConfigurationProvider configurationProvider = null)
        {
            var instance = new TestRunContainerBuilder();
            return instance.CreateContainer(configurationProvider);
        }
    }
}
