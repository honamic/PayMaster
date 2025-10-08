using Honamic.PayMaster.Application.ReceiptRequests.Queries;
using Honamic.PayMaster.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace WebSample.Pages.Recharge
{
    public class ResultModel : PageModel
    {
        private readonly IPayMasterFacade _payMasterFacade;

        public GetPublicReceiptRequestQueryResult? Receipt { get; set; }
        public string? Message { get; set; }

        public ResultModel(IPayMasterFacade payMasterFacade)
        {
            _payMasterFacade = payMasterFacade;
        }

        [FromRoute()]
        public long ReceiptRequestId { get; set; }


        public async Task OnGet(CancellationToken cancellationToken)
        {
            var result = await _payMasterFacade.GetPublicReceiptRequest(new GetPublicReceiptRequestQuery
            {
                Id = ReceiptRequestId
            }, cancellationToken);


            Receipt = result.Data;

            if (Receipt is not null)
            {
                Receipt.GatewayPayments = Receipt.GatewayPayments.OrderByDescending(g => g.RedirectAt).ToList();
            }


            Message = result.Messages.FirstOrDefault()?.Message;

            if (Receipt is null && Message is null)
            {
                Message = "اطلاعات درخواستی یافت نشد.";
            }

        }

    }
}
