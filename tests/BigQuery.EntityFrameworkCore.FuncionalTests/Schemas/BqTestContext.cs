using BigQuery.EntityFrameworkCore;

namespace Schemas
{
    public class BqTestContext : BqContext
    {
        public Data Data { get; set; }
        public Metadata Metadata { get; set; }
    }
}
