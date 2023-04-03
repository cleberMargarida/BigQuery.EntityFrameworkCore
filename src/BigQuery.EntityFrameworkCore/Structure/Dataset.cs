using Google.Cloud.BigQuery.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BigQuery.EntityFrameworkCore
{
    public class Dataset
    {
        internal Dataset()
        {
        }
    }

    public class Dataset<T> : Dataset
    {
        public Dataset() : base()
        {
        }
    }
}
