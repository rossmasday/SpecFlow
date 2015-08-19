using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.BoDi;

namespace TechTalk.SpecFlow
{
    public class Composer
    {
        private readonly CompositionContainer container;

        public Composer()
        {
            const string extensionsDirectory = @"C:\Extensions";

            var defaultCatalog = new CatalogExportProvider(new AssemblyCatalog(typeof(TestRunnerManager).Assembly));

            container = Directory.Exists(extensionsDirectory)
                ? new CompositionContainer(new DirectoryCatalog(extensionsDirectory), defaultCatalog)
                : new CompositionContainer(defaultCatalog);

            defaultCatalog.SourceProvider = container;
        }

        public CompositionContainer Compose<TInstance>(TInstance instance, IObjectContainer objectContainer)
        {
            try
            {
                container.ComposeExportedValue(objectContainer);
                this.container.ComposeParts(instance);
                
            }
            //catch (CompositionException ex)
            //{
            //    //TODO logging?
            //    throw;
            //}
            //TODO Debug code needs to be removed later
            catch (Exception ex)
            {
                var loaderException = ex as ReflectionTypeLoadException;
                if (loaderException == null)
                    throw;

                var info = String.Join("\r\n", loaderException.LoaderExceptions.Select(x => x.Message));
                throw new Exception(info);
            }
            return container;
        }
    }
}
