﻿using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.BoDi;

namespace TechTalk.SpecFlow
{
    public class Composer
    {
        private readonly CompositionContainer container;

        public Composer()
        {
            //TODO this stirng needs a proper place, for now it's just for POC
            const string extensionsDirectory = @"C:\Extensions";

            var defaultCatalog = new CatalogExportProvider(new AssemblyCatalog(typeof(ObjectContainer).Assembly));

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
            catch (CompositionException ex)
            {
                //TODO logging?
                throw;
            }
            return container;
        }
    }
}
