using System.Transactions;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Interfaces;

public interface ITransactionEnricher
{
    void EnrichTransactions(List<Transactions> transactions);
}