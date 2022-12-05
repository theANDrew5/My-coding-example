using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.Services;
using Photoprint.WebSite.Modules;
using Photoprint.WebSite.Views.Checkout;
using System;

namespace Photoprint.WebSite
{
    public partial class DeliveryNew : BaseCheckoutModulePage
    {
        protected DeliveryNew()
        {
            Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedInUser == null || LoggedInUser.IsAnonymous  || ShoppingCart?.Items == null || ShoppingCart.Items.Count == 0)
            {
                if (CurrentPhotolab != null)
                {
                    var lastOrder = OrderService.GetLastOrder(CurrentPhotolab);
                    if (lastOrder != null)
                    {
                        Response.Redirect(new UrlManager { CurrentOrderId = lastOrder.Id }.GetHRefUrl(SiteLinkType.UserOrder));
                        return;
                    }
                }
                RedirectTo(SiteLinkType.UserOrders);
                return;
            }

            if (LoggedInUser.Status == UserStatus.Blocked)
            {
                RedirectTo403();
                return;
            }
        }
    }
}