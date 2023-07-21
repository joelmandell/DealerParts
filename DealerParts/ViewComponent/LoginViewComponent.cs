using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Models;

namespace DealerParts.ViewComponent
{
    public class LoginViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new LoginModel());
        }
    }
}
