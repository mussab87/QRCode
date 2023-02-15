using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using Utility.Core.Logging;

namespace PSS.AccessLayer.Helpers.WS
{
    public class ServiceConfiguration
    {
        public static T GetClient<T>() where T : IClientChannel
        {
            return GetClient<T>(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        /// <summary>
        /// Attempts to get the WCF configuration from the app.config file in the CRM Access Layer library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetClient<T>(string configFilePath) where T : IClientChannel
        {
            FileTrace.WriteMemberEntry();
            try
            {
                var channelType = typeof(T);
                var contractType = channelType.GetInterfaces().First(i => i.Namespace == channelType.Namespace);
                var contractAttribute = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false).First() as ServiceContractAttribute;

                var configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = configFilePath }, ConfigurationUserLevel.None);
                var serviceModelSectionGroup = ServiceModelSectionGroup.GetSectionGroup(configuration);
                var endpoint = serviceModelSectionGroup.Client.Endpoints.OfType<ChannelEndpointElement>().FirstOrDefault(e => e.Contract == contractAttribute.ConfigurationName);
                if (endpoint == null)
                {
                    throw new Exception(string.Format("The end-point '{0}' does not exist. Cannot call service.", contractAttribute.ConfigurationName));
                }
                var channelFactory = new ConfigurationChannelFactory<T>(endpoint.Name, configuration, null);
                var client = channelFactory.CreateChannel();
                return client;
            }
            catch (Exception ex)
            {
                FileTrace.WriteException(ex);
                throw;
            }
        }
    }
}
