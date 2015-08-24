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
        //[Export(typeof(IPluginContainer))]
        internal class SpecFlowStandardKernel : StandardKernel, IObjectContainer
        {
            private readonly SpecFlowStandardKernel baseContainer;
            //private readonly ActivationBlock activationBlock;
            private readonly NamedScope namedScope;

            public SpecFlowStandardKernel()
            {
                this.Rebind<IObjectContainer>().ToConstant(this);
                namedScope = this.CreateNamedScope("Test");
                //activationBlock = new ActivationBlock(this);
            }

            public SpecFlowStandardKernel(IObjectContainer objectContainer)
            {
                
                namedScope = this.CreateNamedScope("Test");
                this.baseContainer = (SpecFlowStandardKernel)objectContainer;
                //activationBlock = new ActivationBlock(this);
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
                    this.Rebind(interfaceType).ToConstant(instance);
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

            public object Resolve(Type typeToResolve, string name = null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return this.baseContainer == null
                        ? this.Get(typeToResolve)
                        : this.baseContainer.Get(typeToResolve);
                }
                return this.baseContainer == null
                    ? this.Get(typeToResolve, name.ToLower())
                    : this.baseContainer.Get(typeToResolve, name.ToLower());
            }

            void IDisposable.Dispose()
            {
                namedScope.Dispose();
               // if (activationBlock != null)
                //this.activationBlock.Dispose();
                base.Dispose();
            }
        }
    }
}
