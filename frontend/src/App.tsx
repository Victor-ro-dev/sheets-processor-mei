import { useMemo } from 'react';
import { FileSpreadsheet, X } from 'lucide-react';
import { CategoryBreakdown } from './components/CategoryBreakdown';
import { KpiCards } from './components/KpiCards';
import { TransactionsTable } from './components/TransactionsTable';
import { UploadDropzone } from './components/UploadDropzone';
import { useStatementDashboard } from './hooks/useStatementDashboard';

export function App() {
  const {
    filters,
    summary,
    transactionsPage,
    isLoading,
    isUploading,
    error,
    feedback,
    uploadFile,
    deleteTransaction,
    updateFilter,
    clearMessages,
  } = useStatementDashboard();

  const availableCategories = useMemo(
    () => summary?.categories.map((c) => c.category) ?? [],
    [summary]
  );

  return (
    <div className="app-shell">
      <header className="topbar">
        <div className="brand">
          <div className="brand-icon">
            <FileSpreadsheet size={22} />
          </div>
          <div>
            <h1>SheetsProcessor MEI</h1>
            <p>Conciliação e categorização inteligente de extratos</p>
          </div>
        </div>
      </header>

      <main className="main-content">
        {(error || feedback) && (
          <div className={`alert ${error ? 'alert-error' : 'alert-success'}`} role="status">
            <span>{error ?? feedback}</span>
            <button type="button" onClick={clearMessages} aria-label="Fechar aviso">
              <X size={16} />
            </button>
          </div>
        )}

        <KpiCards summary={summary} />

        <div className="dashboard-grid">
          <UploadDropzone isUploading={isUploading} onUpload={uploadFile} />
          <CategoryBreakdown categories={summary?.categories ?? []} />
        </div>

        <TransactionsTable
          data={transactionsPage}
          filters={filters}
          categories={availableCategories}
          isLoading={isLoading}
          onFilterChange={updateFilter}
          onDelete={deleteTransaction}
        />
      </main>
    </div>
  );
}

export default App;
