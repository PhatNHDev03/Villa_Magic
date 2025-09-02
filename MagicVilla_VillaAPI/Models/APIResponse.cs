using System.Net;

namespace MagicVilla_VillaAPI.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSucess { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object result { get; set; } = null;
    }
}
