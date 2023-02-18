using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations.Schema
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DatasetAttribute : Attribute
    {
        public DatasetAttribute(string datasetName)
        {
            DatasetName = datasetName;
        }

        public string DatasetName { get; set; }
    }
}
