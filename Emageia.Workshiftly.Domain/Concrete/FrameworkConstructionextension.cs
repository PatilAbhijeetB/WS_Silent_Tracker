using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Domain.Concrete
{
    /// <summary>
    /// Extension methods for the Framewor <see cref="FrameworkConstruction"/>
    /// </summary>
    /// 
    public static class FrameworkConstructionextension
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        //public static FrameworkConstruction UseClientDataStore(this FrameworkConstruction constuction)
        //{

            ////Inject SQLite EF data store
            //constuction.Services.AddDbContext<ClientDataStoreDbContext>(options =>
            //{
            //    //setup connection string
            //    //D:\\FasettoWord.db
            //    options.UseSqlite(constuction.Configuration.GetConnectionString("ClientDataStoreConnection"));
            //  //   options.UseSqlite("D:\\FasettoWord.db");
            //});

            ////add client data store for easy access
            //// scoped so we can inject the scoped DbContext
            //constuction.Services.AddScoped<IClientDataStore>(
            //    provider => new ClientDataStore(provider.GetService<ClientDataStoreDbContext>()));
            ////Return framework for chaining
        //    return constuction;
        //}
        #endregion
    }
}

