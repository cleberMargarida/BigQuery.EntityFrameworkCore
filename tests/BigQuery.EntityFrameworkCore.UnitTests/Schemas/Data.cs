using BigQuery.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schemas
{
    [Dataset("data")]
    public class Data : Dataset<Data>
    {
        public Data() : base()
        {
        }

        public Table<Product> Products { get; set; }
    }

    [Dataset("Metadata")]
    public class Metadata : Dataset<Metadata>
    {
        public Metadata() : base()
        {
        }

        public Table<ProductMetadata> ProductsMetadata { get; set; }
    }
}
