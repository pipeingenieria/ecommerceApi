using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.EntityModels
{
    public class KeyModel
    {    

        public KeyModel() { }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
}
