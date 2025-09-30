using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebSample.Pages.Recharge
{
    public class ResultModel : PageModel
    {
        [FromRoute()]
        public long ReceiptRequestId { get; set; }

        [FromRoute()]
        public string Status { get; set; } = string.Empty;

        public void OnGet()
        {
        }
    }
}
