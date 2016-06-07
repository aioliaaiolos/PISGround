//---------------------------------------------------------------------------------------------------
// <copyright file="CreateOnDispatchServiceAttribute.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Common
{
    using System;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    /// <summary>
    /// Class used to Set the Service Behaviour
    /// </summary>
    public sealed class CreateOnDispatchServiceAttribute : Attribute, IServiceBehavior
    {
        #region Constructors

        /// <summary>Initializes a new instance of the CreateOnDispatchServiceAttribute class.</summary>
        /// <param name="serviceType">Type of the service class.</param>
        public CreateOnDispatchServiceAttribute(Type serviceType)
        {
            _serviceType = serviceType;
        }

        #endregion

        #region IServiceBehavior Members
        /// <summary>
        /// Method to add Binding parameters
        /// </summary>
        /// <param name="serviceDescription">service description</param>
        /// <param name="serviceHostBase">service host</param>
        /// <param name="endpoints">end points</param>
        /// <param name="bindingParameters"> binding paramater</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Applying Dispatch Behaviour
        /// </summary>
        /// <param name="serviceDescription">service description</param>
        /// <param name="serviceHostBase">service host</param>>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            _serviceType.GetConstructor(System.Type.EmptyTypes).Invoke(null);
        }

        /// <summary>
        /// Validate the service
        /// </summary>
        /// <param name="serviceDescription">service description</param>
        /// <param name="serviceHostBase">service host</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion

        #region Private members

        /// <summary>Type of the service class.</summary>
        private Type _serviceType;
        
        #endregion
    }
}
