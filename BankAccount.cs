using System.Globalization;

namespace ReadersWriters;

public struct BankAccount(string accountOwner, double balance)
{
    public string AccountOwner { get; } = accountOwner;
    public double CurBalance { get; private set; } = balance;

    public List<string> TransferHistory { get; } = [];

    private void IncomingTransfer(double amount, BankAccount from)
    {
        CurBalance += amount;
        TransferHistory.Add($"Transfer from {from.AccountOwner}, {amount.ToString("C2", CultureInfo.CreateSpecificCulture("de-AT"))}");
    }

    public void OutgoingTransfer(double amount, BankAccount to)
    {
        CurBalance -= amount;
        TransferHistory.Add($"Transfer to {to.AccountOwner}, {amount}");
        to.IncomingTransfer(amount, this);
    }
}
