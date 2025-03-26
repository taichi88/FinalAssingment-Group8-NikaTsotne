using Microsoft.Extensions.Configuration;

namespace BankingSystem.Application.Constants;

public class AtmConstants
{
    public decimal FeeRate { get; }
    public decimal DailyWithdrawalLimit { get; }

    public AtmConstants(IConfiguration configuration)
    {
        FeeRate = configuration.GetValue<decimal>("Atm:FeeRate");
        DailyWithdrawalLimit = configuration.GetValue<decimal>("Atm:DailyWithdrawalLimit");
    }
}
