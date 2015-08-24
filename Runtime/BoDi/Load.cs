using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.BoDi
{
    public class Load
    {
        private readonly IObjectContainer objectContainer;

        public Load(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;
        }

        //public IDictionary<string, IRuntimePlugin> RuntimePlugins()
        //{
            


        //}

    }
}
