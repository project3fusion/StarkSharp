

using System;

namespace StarkSharp.Accounts
{
    public class AccountDeploymentResult : SentTransaction
    {
        public Account Account { get; }

        public AccountDeploymentResult(Account account)
            : base()
        {
            Account = account ?? throw new ArgumentNullException(nameof(account), "Parameter account cannot be None in AccountDeploymentResult.");
        }
    }
}
