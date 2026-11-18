using System.Transactions;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Services;

public class TransactionEnricher : ITransactionEnricher
{
    public void EnrichTransactions(List<Transactions> transactions)
    {
        foreach (var transaction in transactions)
        {
            if(ApplyTaxRate(transaction)) continue;
            if(ApplyPersonalTransaction(transaction)) continue;
            ApplyFlowRule(transaction);
        }
    }

    private bool ApplyTaxRate(Transactions transaction)
    {
        if(transaction.Description.Contains("DAS", StringComparison.OrdinalIgnoreCase) || transaction.Description.Contains("RECEITA FEDERAL", StringComparison.OrdinalIgnoreCase))
        {
            transaction.Category = "Imposto MEI";
            transaction.PersonalUse = false;
            return true;
        }
        return false;
    }

    private bool ApplyPersonalTransaction(Transactions transaction)
    {
        bool isWeekend = transaction.Date.DayOfWeek == DayOfWeek.Saturday || 
        transaction.Date.DayOfWeek == DayOfWeek.Sunday;

        if (isWeekend &&
            (transaction.Description.Contains("IFOOD", StringComparison.OrdinalIgnoreCase) ||
             transaction.Description.Contains("UBER", StringComparison.OrdinalIgnoreCase) ||
             transaction.Description.Contains("MCDONALDS", StringComparison.OrdinalIgnoreCase)))
        {
            
            transaction.Category = "Gasto Pessoal";
            transaction.PersonalUse = true;
            return true;
        }
        return false;
    }

    private void ApplyFlowRule(Transactions transaction)
    {
        transaction.Category = transaction.Value > 0 ? "Receita Operacional" : "Despesa Operacional";
        transaction.PersonalUse = false;
    }
}