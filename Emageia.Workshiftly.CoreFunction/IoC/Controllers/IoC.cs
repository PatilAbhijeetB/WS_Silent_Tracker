using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers
{
    /// <summary>
    /// The Ioc Container for our appliction
    /// </summary>
    public static class IoC
    {

        #region Public Properties

        /// <summary>
        /// The Kernel for our IoC Container
        /// </summary>
        public static IKernel Kernal { get; private set; } = new StandardKernel();

        //public IoC(IClientDataStore clientDataStore)
        //{
        //    var nn = clientDataStore;
        //}
        //public static IClientDataStore ClientDataStore()
        //{

        //    var ni = new IoCDatabaseInject(new IClientDataStore());

        //}
        #endregion

        #region Construciton

        /// <summary>
        /// Set Up the IoC container , binds all information required and is ready for use
        /// Check ===Called your application starts up to ensure all services can be found
        /// </summary>
        public static void Setup()
        {
            //Bind All required view models
            NewMethod();
        }

        private static void NewMethod()
        {
            BindViewModels();
        }

        private static void BindViewModels()
        {
            // Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
        }
        #endregion

        /// <summary>
        /// Get Service from the Ioc Using Generics
        /// </summary>
        /// <typeparam name="T"> The Type to Get </typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernal.Get<T>();
        }
    }
}
