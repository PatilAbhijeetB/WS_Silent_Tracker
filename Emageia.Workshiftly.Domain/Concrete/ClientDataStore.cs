
using Emageia.Workshiftly.Domain.Interface;
using Emageia.Workshiftly.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Domain.Concrete
{
    /// <summary>
    /// Stores and retrieves information about the Client application
    /// SQLite database
    /// </summary>
    public class ClientDataStore : IClientDataStore
    {
        #region Public Properties

        /// <summary>
        /// database context for the client data store
        /// </summary>
        protected ClientDataStoreDbContext _DbContext;


        #endregion

        #region Constructor
        //public ClientDataStore(ClientDataStoreDbContext dbContext)
        //{
        //    _DbContext = dbContext;
        //}

        public ClientDataStore()
        {
            _DbContext = new ClientDataStoreDbContext();
        }
        #endregion


        #region Interface
        /// <summary>
        /// Make sure the Client Database Install correnctly
        /// </summary>
        /// <returns>Returns a task that will finish onece setup is complete</returns>
        public async Task EnsureDataStoreAsync()
        {
            //Meke sure Database exists and create
           
           // await _DbContext.Database.EnsureCreatedAsync();
        }

        /// <summary>
        /// get the stored logging credentials
        /// </summary>
        /// <returns> Returns the login credentials if the exist , null </returns>
        public Task<LoginCredentialsDataModel> GetLoginCredentialsAsync()
        {
            return Task.FromResult(_DbContext.LoginCredentials.FirstOrDefault());
        }

        /// <summary>
        /// Stores th gieve login credentials
        /// </summary>
        /// <param name="loginCredentials"></param>
        /// <returns> Returnd a task that finish once the save is complete</returns>
        public async Task SaveLoginCredentialsAsync(LoginCredentialsDataModel loginCredentials)
        {
            //revome old loggin
            _DbContext.LoginCredentials.RemoveRange(_DbContext.LoginCredentials);

            //add new 
            _DbContext.LoginCredentials.Add(loginCredentials);

            //save changes apply to db
            await _DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Stores th gieve login credentials
        /// </summary>
        /// <param name="loginCredentials"></param>
        /// <returns> Returnd a task that finish once the save is complete</returns>
        public async Task SavesLoginCredentialsAsync(List<LogAppData> loginCredentials)
        {
            //revome old loggin

            _DbContext.LogAppDatas.RemoveRange(_DbContext.LogAppDatas);
            //add new 
            _DbContext.LogAppDatas.AddRange(loginCredentials);

            //save changes apply to db
            await _DbContext.SaveChangesAsync();
        }

        //public Task<bool> HasCredentialsAsync()
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Determines if the current user has logged in credentials
        /// </summary>
        public async Task<bool> HasCredentialsAsync()
        {
            return await GetLoginCredentialsAsync() != null;
        }

        #endregion

        //#region Methods

        ///// <summary>
        ///// Gets the stored login credentials for this client
        ///// </summary>
        ///// <returns>Returns the login credentials if they exist, or null if none exist</returns>
        ///// <exception cref="NotImplementedException"></exception>
        //public LoginCredentialsDataModel GetLoginCredentials()
        //{
        //     throw new NotImplementedException();
        //}

        //#endregion

    }
}
