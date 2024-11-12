using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelUnloader.Models
{
    public class ModelRecord
    {
        public string ModelName { get; set; }
        public int ModelVersion { get; set; }
        public string ModelDescription { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string PublishDate { get; set; }
    }
}
