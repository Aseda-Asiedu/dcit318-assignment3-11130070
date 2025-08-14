using System;
using System.Collections.Generic;

namespace FinanceManagement
{
    
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processed GHS{transaction.Amount:F2} for '{transaction.Category}' on {transaction.Date:d}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processed GHS{transaction.Amount:F2} for '{transaction.Category}' on {transaction.Date:d}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Processed GHS{transaction.Amount:F2} for '{transaction.Category}' on {transaction.Date:d}.");
        }
    }
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }
    }
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New Balance: GHS{Balance:F2}");
        }
    }
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            var account = new SavingsAccount("ASeda-29393393939", 1000m);

            var t1 = new Transaction(1, DateTime.Now, 176.50m, "Fuel");
            var t2 = new Transaction(2, DateTime.Now, 540m, "Electrical Bill");
            var t3 = new Transaction(3, DateTime.Now, 27m, "Rent");

            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            p1.Process(t1);
            p2.Process(t2);
            p3.Process(t3);

            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);

            
            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine($"Final Balance for {account.AccountNumber}: GHS{account.Balance:F2}");
            Console.WriteLine("\nAll Transactions:");
            foreach (var t in _transactions)
                Console.WriteLine($"  #{t.Id} {t.Category} - GHS{t.Amount:F2} on {t.Date:g}");
        }
    }

    public class Program
    {
        public static void Main()
        {
            new FinanceApp().Run();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
