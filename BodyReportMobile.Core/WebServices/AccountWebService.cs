using BodyReportMobile.Core.Framework;
using Message.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.WebServices
{
    public static class AccountWebService
    {
        public static async Task<bool> RegisterAccountAsync(string userName, string email, string password)
        {
            RegisterAccount registerAccount = new RegisterAccount()
            {
                UserName = userName,
                Email = email,
                Password = password
            };
            return await HttpConnector.Instance.PostAsync<RegisterAccount, bool>("Api/Account/Register", registerAccount, true);
        }
    }
}
