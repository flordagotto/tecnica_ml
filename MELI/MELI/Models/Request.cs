using System;

namespace MELI.Models
{
    public partial class Request
    {
        public string Endpoint { get; set; }
        public string UrlRequest { get; set; }
        public DateTime Date { get; set; }
        public int ResponseCodeProxy { get; set; }
        public int ResponseCodeApi { get; set; }
        public string SourceIp { get; set; }
        public string Method { get; set; }
        public int Id { get; set; }
    }
}
