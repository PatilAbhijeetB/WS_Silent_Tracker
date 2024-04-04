using Emageia.Workshiftly.CoreFunction.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Interfaces
{
    /// <summary>
    /// Stores and retrieves information about the Client application
    /// 
    /// </summary>
    public interface IClientDataStores
    {
        /// <summary>
        /// Determines if the current user has logged in credentials
        /// </summary>
        Task<bool> HasCredentialsAsync();

        /// <summary>
        /// Make sure the Client Database Install correnctly
        /// </summary>
        /// <returns>Returns a task that will finish onece setup is complete</returns>
        Task EnsureDataStoreAsync();

        /// <summary>
        /// get the stored logging credentials
        /// </summary>
        /// <returns> Returns the login credentials if the exist , null </returns>
        Task<LoginCredentialsDataModel> GetLoginCredentialsAsync();

        /// <summary>
        /// Stores th gieve login credentials
        /// </summary>
        /// <param name="loginCredentials"></param>
        /// <returns> Returnd a task that finish once the save is complete</returns>
        Task SaveLoginCredentialsAsync(LoginCredentialsDataModel loginCredentials);





    }
}
