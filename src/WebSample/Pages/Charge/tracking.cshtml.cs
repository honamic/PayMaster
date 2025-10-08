using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebSample.Pages.Charge
{
    public class TrackingModel : PageModel
    {
        public void OnGet()
        {
        }


        [BindProperty]
        public string? ReceiptRequestId { get; set; }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(ReceiptRequestId))
            {
                ModelState.AddModelError("ReceiptRequestId", "شناسه قبض الزامی است.");
                return Page();
            }

            return RedirectToPage("Result", new { ReceiptRequestId });
        }

    }
}
