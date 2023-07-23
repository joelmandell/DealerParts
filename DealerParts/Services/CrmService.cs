using DealerParts.Models;
using System.Data;

namespace DealerParts.Services
{
    public class CrmService
    {
        public ValueTask<CrmResult> SignIn(string username, string password)
        {
            string? role = "";
            string? crmUserName = "";

            switch (password)
            {
                case "admin":
                    role = "Dealership Administrators";
                    crmUserName = "Got Rootson";
                    break;
                case "customer":
                    role = "Retail Customers";
                    crmUserName = "Sir Buy Alot";
                    break;
                default:
                    role = "Parts Department Staff";
                    crmUserName = "Mr Always Employee of the month";
                    break;
            }

            return new ValueTask<CrmResult>(new CrmResult() { Success = true, Role = role, UserName = crmUserName});
        }
    }
}
