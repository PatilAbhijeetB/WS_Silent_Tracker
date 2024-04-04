using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class UserSessions
    {
        public string id { get; set; }
        public string UserId { get; set; } //= "63102b27-5ac7-4f93-8706-044ef00aee17";
        public string companyId { get; set; } //="c568e7e2-6357-44f2-b2eb-8cc13b1a63f8";
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string authToken { get; set; }// = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYzMTAyYjI3LTVhYzctNGY5My04NzA2LTA0NGVmMDBhZWUxNyIsImVtYWlsIjoic2lyaXNlbmFAbWFpbGluYXRvci5jb20iLCJmaXJzdE5hbWUiOiJLdW1hcmEiLCJsYXN0TmFtZSI6IlNpcmlzZW5hIiwicm9sZSI6eyJpZCI6IjIxYmRmYTM1LTIyY2UtNDQyNi04NzMwLWZjNzRlZjhjZDY0OCIsIm5hbWUiOiJVU0VSIiwiZGVzY3JpcHRpb24iOiJ1c2VyIiwicGVybWlzc2lvbnMiOiJ7XCJrZXlcIjpcInVzZXJcIiwgXCJncmFudFR5cGVcIjogXCJhbGxcIiwgXCJkZXNjcmlwdGlvblwiOiBcIlwifSIsImlzQWN0aXZlIjoxLCJjcmVhdGVkQXQiOjE2MjA2NDMxNDQsInVwZGF0ZWRBdCI6MTYyMDY0MzE0NH0sImlzQWN0aXZlIjp0cnVlLCJpc0NsaWVudEFjdGl2ZSI6dHJ1ZSwiaXNBbGxvd09mZmxpbmVUYXNrIjpmYWxzZSwiaWF0IjoxNjQ4NDQwMDIzfQ.xaxwzE7efdoDEIHEoE8Ng5MY0--tNTVuXjqwlRUCXEI";
        public bool isActive { get; set; } = true;
        public bool isClientActive { get; set; } = true;
        public bool isAllowOfflineTask { get; set; } = true;
    }
}
