import { memo, useMemo } from 'react';
import type { CategoryBreakdown as CategoryBreakdownItem } from '../types/statement';
import { formatCurrency } from '../utils/formatters';

interface CategoryBreakdownProps {
  categories: CategoryBreakdownItem[];
}

export const CategoryBreakdown = memo(function CategoryBreakdown({
  categories,
}: CategoryBreakdownProps) {
  const maxAbsTotal = useMemo(
    () => Math.max(1, ...categories.map((c) => Math.abs(c.totalValue))),
    [categories]
  );

  if (categories.length === 0) {
    return (
      <section className="panel">
        <h2>Distribuição por Categoria</h2>
        <p className="empty-text">Nenhuma transação importada ainda.</p>
      </section>
    );
  }

  return (
    <section className="panel">
      <h2>Distribuição por Categoria</h2>
      <div className="category-list">
        {categories.map((item) => {
          const widthPercent = Math.round((Math.abs(item.totalValue) / maxAbsTotal) * 100);
          const isPositive = item.totalValue >= 0;

          return (
            <div key={item.category} className="category-row">
              <div className="category-info">
                <span className="category-name">
                  {item.category} <small>({item.count})</small>
                </span>
                <span className={isPositive ? 'positive' : 'negative'}>
                  {formatCurrency(item.totalValue)}
                </span>
              </div>
              <div className="progress-track">
                <div
                  className={`progress-fill ${isPositive ? 'fill-positive' : 'fill-negative'}`}
                  style={{ width: `${widthPercent}%` }}
                />
              </div>
            </div>
          );
        })}
      </div>
    </section>
  );
});
