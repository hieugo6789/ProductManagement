namespace SE170311.Lab3.Payload.Response.Products
{
    public class GetProductResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
        public object? data { get; set; }
    }
}
