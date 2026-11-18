import { memo } from 'react';
import { ArrowDownRight, ArrowUpRight, Landmark, Wallet } from 'lucide-react';
import type { StatementSummary } from '../types/statement';
import { formatCurrency } from '../utils/formatters';

interface KpiCardsProps {
  summary: StatementSummary | null;
}

export const KpiCards = memo(function KpiCards({ summary }: KpiCardsProps) {
  const revenue = summary?.totalRevenue ?? 0;
  const expenses = summary?.totalExpenses ?? 0;
  const netBalance = summary?.netBalance ?? 0;
  const taxPaid = summary?.taxPaid ?? 0;
  const personalExpenses = summary?.personalExpenses ?? 0;

  return (
    <section className="kpi-grid" aria-label="Resumo financeiro">
      <article className="kpi-card">
        <div className="kpi-header">
          <span>Receita Operacional</span>
          <ArrowUpRight className="icon-positive" size={18} />
        </div>
        <strong className="kpi-value positive">{formatCurrency(revenue)}</strong>
        <small className="kpi-meta">{summary?.totalCount ?? 0} lançamentos totais</small>
      </article>

      <article className="kpi-card">
        <div className="kpi-header">
          <span>Despesas Totais</span>
          <ArrowDownRight className="icon-negative" size={18} />
        </div>
        <strong className="kpi-value negative">{formatCurrency(expenses)}</strong>
        <small className="kpi-meta">Uso pessoal: {formatCurrency(personalExpenses)}</small>
      </article>

      <article className="kpi-card">
        <div className="kpi-header">
          <span>Saldo Líquido</span>
          <Wallet className="icon-neutral" size={18} />
        </div>
        <strong className={`kpi-value ${netBalance >= 0 ? 'positive' : 'negative'}`}>
          {formatCurrency(netBalance)}
        </strong>
        <small className="kpi-meta">Fluxo consolidado</small>
      </article>

      <article className="kpi-card">
        <div className="kpi-header">
          <span>Impostos MEI (DAS)</span>
          <Landmark className="icon-amber" size={18} />
        </div>
        <strong className="kpi-value">{formatCurrency(taxPaid)}</strong>
        <small className="kpi-meta">Guias identificadas</small>
      </article>
    </section>
  );
});
