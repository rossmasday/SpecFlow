using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Activation.Blocks;
using Ninject.Activation.Caching;
using Ninject.Extensions.NamedScope;
using TechTalk.SpecFlow.BoDi;

namespace TechTalk.Specflow.Extensions
{
    class NinjectPlugin
    {
        //TODO RA Setup notes for final build structure
        //This ninject plugin goes in an external assembly probably TechTalk.SpecFlow.Extensions.NinjectPlugin
        //That new assemby will be in its own repo and have a ref to TechTalk.SpecFlow.Extensions
        //This assembly TechTalk.SpecFlow.Extensions will have only the following two files in it
        //IObjectContainer & IPluginContainerFactory
        //Need to find a way to get specflow to ref this assembly
        //Then this assembly will be located in separate repo
        //This assembly may be used for other plugins
        internal class SpecFlowStandardKernel : StandardKernel, IObjectContainer
        {
            private readonly SpecFlowStandardKernel baseContainer;
            private readonly ActivationBlock activationBlock;

            public SpecFlowStandardKernel()
            {
                activationBlock = new ActivationBlock(this);
                this.Rebind<IObjectContainer>().ToConstant(this);
            }

            public SpecFlowStandardKernel(IObjectContainer objectContainer)
            {
                this.baseContainer = (SpecFlowStandardKernel)objectContainer;
                activationBlock = new ActivationBlock(this);
            }

            public void RegisterTypeAs<TType, TInterface>(string name = null) where TType : class, TInterface
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    this.Rebind<TInterface>().To<TType>().InSingletonScope();
                    return;
                }
                this.Bind<TInterface>().To<TType>().InSingletonScope().Named(name.ToLower());
            }

            public void RegisterInstanceAs<TInterface>(TInterface instance, string name = null) where TInterface : class
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    this.Rebind<TInterface>().ToConstant(instance);
                    return;
                }
                this.Bind<TInterface>().ToConstant(instance).Named(name.ToLower());
            }

            public void RegisterInstanceAs(object instance, Type interfaceType, string name = null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    this.Rebind(interfaceType).ToConstant(instance).InSingletonScope();
                    return;
                }
                this.Bind(interfaceType).ToConstant(instance).Named(name.ToLower());
            }

            public void RegisterTypeAs(Type implementationType, Type interfaceType, string name = null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    this.Rebind(interfaceType).To(implementationType).InSingletonScope();
                    return;
                }
                this.Rebind(interfaceType).To(implementationType).InSingletonScope().Named(name.ToLower());
            }

            public T Resolve<T>()
            {
                return this.baseContainer == null ? 
                    this.Get<T>() : 
                    this.baseContainer.Get<T>();
            }

            public T Resolve<T>(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return this.baseContainer == null
                        ? this.Get<T>()
                        : this.baseContainer.Get<T>();
                }
                return this.baseContainer == null
                    ? this.Get<T>(name.ToLower())
                    : this.baseContainer.Get<T>(name.ToLower());
            }

            //This is primarily used by scenario context, it has more logic than the other because it needs to work as it always has
            public object Resolve(Type typeToResolve, string name = null)
            {
                object tInstance;
                if (string.IsNullOrWhiteSpace(name))
                {
                    tInstance = this.TryGet(typeToResolve);
                    if (tInstance != null)
                    {
                        Rebind(typeToResolve).ToConstant(tInstance).InSingletonScope();
                        return tInstance;
                    }
                    tInstance = this.baseContainer.TryGet(typeToResolve);
                    //This will cause a fail, neither class could resolve it, the Ninject fail will be more meaningful
                    if (tInstance == null) 
                        return this.Get(typeToResolve);

                    this.baseContainer.Rebind(typeToResolve).ToConstant(tInstance).InSingletonScope();
                    return tInstance;
                }
                //TODO this needs the base container logic as above, but for now we're ok
                tInstance = this.TryGet(typeToResolve, name.ToLower());
                Rebind(typeToResolve).ToConstant(tInstance).InSingletonScope().Named(name.ToLower());
                return tInstance;
            }

            void IDisposable.Dispose()
            {
                //TODO this should displose my objects but isnt
                if (activationBlock != null)
                    this.activationBlock.Dispose();
            }
        }
    }
}
