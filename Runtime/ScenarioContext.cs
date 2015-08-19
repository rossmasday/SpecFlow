using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.BoDi;
using TechTalk.SpecFlow.Infrastructure;

#if SILVERLIGHT
using TechTalk.SpecFlow.Compatibility;
#endif

namespace TechTalk.SpecFlow
{
    public class ScenarioContext : SpecFlowContext
    {
        private static ScenarioContext current;
        public static ScenarioContext Current
        {
            get
            {
                if (current == null)
                {
                    Debug.WriteLine("Accessing NULL ScenarioContext");
                }
                return current;
            }
            internal set { current = value; }
        }

        public ScenarioInfo ScenarioInfo { get; private set; }
        public ScenarioBlock CurrentScenarioBlock { get; internal set; }
        public Exception TestError { get; internal set; }

        internal TestStatus TestStatus { get; set; }
        internal List<string> PendingSteps { get; private set; }
        internal List<StepInstance> MissingSteps { get; private set; }
        internal Stopwatch Stopwatch { get; private set; }

        internal ITestRunner TestRunner { get; private set; }

        [Import(typeof (IPluginContainer), AllowDefault = true)]
        private IObjectContainer pluginContainer;

        private readonly IObjectContainer objectContainer;

        internal ScenarioContext(ScenarioInfo scenarioInfo, ITestRunner testRunner, IObjectContainer parentContainer)
        {
            var composer = new Composer();
            composer.Compose(this, parentContainer);

            this.objectContainer = parentContainer == null
                ? new ObjectContainer()
                : pluginContainer ?? new ObjectContainer(parentContainer);

            TestRunner = testRunner;

            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            CurrentScenarioBlock = ScenarioBlock.None;
            ScenarioInfo = scenarioInfo;
            TestStatus = TestStatus.OK;
            PendingSteps = new List<string>();
            MissingSteps = new List<StepInstance>();
        }

        public void Pending()
        {
            TestRunner.Pending();
        }

        public object GetBindingInstance(Type bindingType)
        {
            //TODO This is special just for scenario context because it needs to add to plugin not base
            //return  objectContainer.ResolveNew(bindingType);
            return objectContainer.Resolve(bindingType);
        }

        internal void SetBindingInstance(Type bindingType, object instance)
        {
            objectContainer.RegisterInstanceAs(instance, bindingType);
        }

        protected override void Dispose()
        {
            base.Dispose();
            //TODO this does not work with plugin containers because this calls all items in the container and executes dispose on them
            //this is nonstandard IOC behaviour i cannot mimick
            objectContainer.Dispose();
        }
    }
}