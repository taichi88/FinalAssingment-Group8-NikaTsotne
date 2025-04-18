using Microsoft.Extensions.Configuration;

namespace BankingSystem.Application.Constants;

public class TransactionConstants
{
    public decimal TransferToOthersFeeRate { get; }
    public decimal TransferToOthersBaseFee { get; }
    public decimal ToMyAccountFee { get; }
    public string BaseCurrency { get; }

    public TransactionConstants(IConfiguration configuration)
    {
        TransferToOthersFeeRate = configuration.GetValue<decimal>("Transaction:TransferToOthersFeeRate");
        TransferToOthersBaseFee = configuration.GetValue<decimal>("Transaction:TransferToOthersBaseFee");
        ToMyAccountFee = configuration.GetValue<decimal>("Transaction:ToMyAccountFee");
        BaseCurrency = configuration.GetValue<string>("Transaction:BaseCurrency") ?? "GEL";
    }


}
