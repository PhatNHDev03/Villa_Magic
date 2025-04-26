using System.Net;

namespace MagicVilla_VillaAPI.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSucess { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public object result { get; set; }
    }
}
