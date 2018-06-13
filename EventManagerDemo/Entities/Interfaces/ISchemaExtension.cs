using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagerDemo.Models
{
    public interface ISchemaExtension
    {
        string id { get; }
        IEnumerable<JsonHelpers.Property1> properties { get; }
        string status { get; set; }
        string description { get; }


    }
}
