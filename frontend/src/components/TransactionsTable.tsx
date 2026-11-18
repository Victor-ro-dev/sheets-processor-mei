import { memo } from 'react';
import { ChevronLeft, ChevronRight, Search, Trash2 } from 'lucide-react';
import type { PagedResult, Transaction, TransactionFilters } from '../types/statement';
import { formatCurrency, formatDate } from '../utils/formatters';

interface TransactionRowProps {
  transaction: Transaction;
  onDelete: (id: string) => Promise<void>;
}

const TransactionRow = memo(function TransactionRow({
  transaction,
  onDelete,
}: TransactionRowProps) {
  const isPositive = transaction.value >= 0;

  return (
    <tr>
      <td className="mono">{formatDate(transaction.date)}</td>
      <td className="desc-cell" title={transaction.description}>
        {transaction.description}
      </td>
      <td>
        <span className="badge">{transaction.category}</span>
      </td>
      <td>
        <span className={`tag ${transaction.personalUse ? 'tag-personal' : 'tag-mei'}`}>
          {transaction.personalUse ? 'Pessoal' : 'MEI'}
        </span>
      </td>
      <td className={`amount-cell ${isPositive ? 'positive' : 'negative'}`}>
        {formatCurrency(transaction.value)}
      </td>
      <td className="actions-cell">
        <button
          type="button"
          className="btn-icon"
          title="Remover transação"
          aria-label={`Remover ${transaction.description}`}
          onClick={() => void onDelete(transaction.id)}
        >
          <Trash2 size={15} />
        </button>
      </td>
    </tr>
  );
});

interface TransactionsTableProps {
  data: PagedResult<Transaction> | null;
  filters: TransactionFilters;
  categories: string[];
  isLoading: boolean;
  onFilterChange: <K extends keyof TransactionFilters>(
    key: K,
    value: TransactionFilters[K]
  ) => void;
  onDelete: (id: string) => Promise<void>;
}

export const TransactionsTable = memo(function TransactionsTable({
  data,
  filters,
  categories,
  isLoading,
  onFilterChange,
  onDelete,
}: TransactionsTableProps) {
  const items = data?.items ?? [];
  const totalPages = Math.max(1, data?.totalPages ?? 1);

  return (
    <section className="panel">
      <div className="table-toolbar">
        <h2>Transações ({data?.totalCount ?? 0})</h2>

        <div className="filters-row">
          <div className="search-box">
            <Search size={16} />
            <input
              type="search"
              placeholder="Buscar descrição..."
              value={filters.search}
              onChange={(e) => onFilterChange('search', e.target.value)}
            />
          </div>

          <select
            aria-label="Filtrar por categoria"
            value={filters.category}
            onChange={(e) => onFilterChange('category', e.target.value)}
          >
            <option value="">Todas categorias</option>
            {categories.map((cat) => (
              <option key={cat} value={cat}>
                {cat}
              </option>
            ))}
          </select>

          <select
            aria-label="Filtrar por uso"
            value={filters.personalUse}
            onChange={(e) =>
              onFilterChange('personalUse', e.target.value as TransactionFilters['personalUse'])
            }
          >
            <option value="all">MEI + Pessoal</option>
            <option value="false">Apenas MEI</option>
            <option value="true">Apenas Pessoal</option>
          </select>
        </div>
      </div>

      <div className="table-wrapper">
        <table>
          <thead>
            <tr>
              <th>Data</th>
              <th>Descrição</th>
              <th>Categoria</th>
              <th>Perfil</th>
              <th className="amount-cell">Valor</th>
              <th className="actions-cell">Ação</th>
            </tr>
          </thead>
          <tbody>
            {isLoading ? (
              <tr>
                <td colSpan={6} className="table-empty">
                  Carregando transações...
                </td>
              </tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={6} className="table-empty">
                  Nenhuma transação encontrada para os filtros selecionados.
                </td>
              </tr>
            ) : (
              items.map((tx) => (
                <TransactionRow key={tx.id} transaction={tx} onDelete={onDelete} />
              ))
            )}
          </tbody>
        </table>
      </div>

      <div className="pagination">
        <span>
          Página {filters.page} de {totalPages}
        </span>
        <div className="pagination-buttons">
          <button
            type="button"
            className="btn-secondary"
            disabled={filters.page <= 1}
            onClick={() => onFilterChange('page', filters.page - 1)}
          >
            <ChevronLeft size={16} />
            Anterior
          </button>
          <button
            type="button"
            className="btn-secondary"
            disabled={filters.page >= totalPages}
            onClick={() => onFilterChange('page', filters.page + 1)}
          >
            Próxima
            <ChevronRight size={16} />
          </button>
        </div>
      </div>
    </section>
  );
});
