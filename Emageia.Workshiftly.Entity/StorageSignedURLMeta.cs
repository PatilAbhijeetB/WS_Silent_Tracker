using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class StorageSignedURLMeta
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]

        public string ObjectType { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string Key { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string ContentType { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string ContentEncoding { get; set; }

        [Column(TypeName = "VARCHAR")]

        public bool IsSynced { get; set; }

        [Column(TypeName = "VARCHAR")]
        public Int32 Expiration { get; set; }
        public Int32 ExpirationTimeStamp { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string Url { get; set; }


       // [Column(TypeName = "VARCHAR")]
        public bool Completed
        { get; set;
        }
        [Column(TypeName = "VARCHAR")]
        public string Action { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string Data { get; set; }
    }
}
