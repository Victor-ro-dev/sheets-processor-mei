using System.Transactions;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Interfaces;

public interface ITransactionEricher
{
    void EnrichTransactions(List<Transaction> transactions);
}