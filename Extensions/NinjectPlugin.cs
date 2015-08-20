using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Activation.Blocks;
using Ninject.Activation.Caching;
using TechTalk.SpecFlow.BoDi;

namespace TechTalk.Specflow.Extensions
{
    class NinjectPlugin
    {
        [Export(typeof(IPluginContainer))]
        internal class SpecFlowStandardKernel : StandardKernel, IObjectContainer
        {
            private readonly IObjectContainer baseContainer;
            private readonly ActivationBlock activationBlock;

            [ImportingConstructor]
            public SpecFlowStandardKernel(IObjectContainer objectContainer)
            {
                this.baseContainer = objectContainer;
                this.Bind<IObjectContainer>().ToConstant(this);
                activationBlock = new ActivationBlock(this);
            }

            public void RegisterTypeAs<TType, TInterface>(string name = null) where TType : class, TInterface
            {
                this.baseContainer.RegisterTypeAs<TType, TInterface>(name);
            }

            public void RegisterInstanceAs<TInterface>(TInterface instance, string name = null) where TInterface : class
            {
                this.baseContainer.RegisterInstanceAs(instance, name);
            }

            public void RegisterInstanceAs(object instance, Type interfaceType, string name = null)
            {
                 this.baseContainer.RegisterInstanceAs(instance, interfaceType, name);
            }

            public T Resolve<T>()
            {
                return this.baseContainer.Resolve<T>();
            }

            public T Resolve<T>(string name)
            {
               return this.baseContainer.Resolve<T>(name);
            }

            public object Resolve(Type typeToResolve, string name = null)
            {
                return this.baseContainer.Resolve(typeToResolve, name);
            }

            void IDisposable.Dispose()
            {
                this.activationBlock.Dispose();
                ((IScenarioContextContainer)baseContainer).DisposeScenarioContext();
            }
        }
    }
}
