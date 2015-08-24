using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;

namespace TechTalk.SpecFlow.BoDi
{
    internal class RegisterConfiguration
    {
        private readonly IObjectContainer objectContainer;

        public RegisterConfiguration(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;
        }

        public void RegisterFromConfiguration()
        {
            var section = (BoDiConfigurationSection)ConfigurationManager.GetSection("boDi");
            if (section == null)
                return;

            RegisterFromConfiguration(section.Registrations);
        }

        public void RegisterFromConfiguration(ContainerRegistrationCollection containerRegistrationCollection)
        {
            if (containerRegistrationCollection == null)
                return;

            foreach (ContainerRegistrationConfigElement registrationConfigElement in containerRegistrationCollection)
            {
                RegisterFromConfiguration(registrationConfigElement);
            }
        }

        private void RegisterFromConfiguration(ContainerRegistrationConfigElement registrationConfigElement)
        {
            Type interfaceType = Type.GetType(registrationConfigElement.Interface, true);
            Type implementationType = Type.GetType(registrationConfigElement.Implementation, true);

            objectContainer.RegisterTypeAs(implementationType, interfaceType, string.IsNullOrEmpty(registrationConfigElement.Name) ? null : registrationConfigElement.Name);
        }
    }
}
