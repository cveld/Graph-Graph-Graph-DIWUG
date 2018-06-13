using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManagerDemo.Models.JsonHelpers
{
    public class GraphCollection<T>
    {
        public string odatacontext { get; set; }
        public T[] value { get; set; }
    }

}