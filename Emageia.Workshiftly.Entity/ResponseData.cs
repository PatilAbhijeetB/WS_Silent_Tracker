using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public  class ResponseData<T>
    {
        public List<T> requestData { get; set; }
        public string userId { get; set; }
    }
}
