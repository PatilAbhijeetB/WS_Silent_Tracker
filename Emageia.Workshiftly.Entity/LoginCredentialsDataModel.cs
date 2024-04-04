using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class LoginCredentialsDataModel
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string UserName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string FirstName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string LastName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string Email { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string Token { get; set; }
    }
}
