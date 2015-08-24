﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.BoDi
{

    public interface IPluginContainerFactory
    {
        IObjectContainer CreateContainer();
        IObjectContainer CreateContainer(IObjectContainer container);
    }
}
