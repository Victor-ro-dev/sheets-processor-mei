import { useCallback, useEffect, useRef, useState } from 'react';
import { statementApi } from '../services/api';
import { useDebounce } from './useDebounce';
import type {
  PagedResult,
  StatementSummary,
  Transaction,
  TransactionFilters,
} from '../types/statement';

const INITIAL_FILTERS: TransactionFilters = {
  category: '',
  personalUse: 'all',
  search: '',
  page: 1,
  pageSize: 15,
};

export function useStatementDashboard() {
  const [filters, setFilters] = useState<TransactionFilters>(INITIAL_FILTERS);
  const debouncedSearch = useDebounce(filters.search, 300);

  const [summary, setSummary] = useState<StatementSummary | null>(null);
  const [transactionsPage, setTransactionsPage] = useState<PagedResult<Transaction> | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isUploading, setIsUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [feedback, setFeedback] = useState<string | null>(null);

  const summaryAbortRef = useRef<AbortController | null>(null);
  const listAbortRef = useRef<AbortController | null>(null);

  const fetchSummary = useCallback(async () => {
    summaryAbortRef.current?.abort();
    const controller = new AbortController();
    summaryAbortRef.current = controller;

    try {
      const data = await statementApi.getSummary(controller.signal);
      setSummary(data);
    } catch (err) {
      if ((err as Error).name !== 'AbortError') {
        setError((err as Error).message);
      }
    }
  }, []);

  const fetchTransactions = useCallback(async (activeFilters: TransactionFilters) => {
    listAbortRef.current?.abort();
    const controller = new AbortController();
    listAbortRef.current = controller;
    setIsLoading(true);

    try {
      const data = await statementApi.getTransactions(activeFilters, controller.signal);
      setTransactionsPage(data);
      setError(null);
    } catch (err) {
      if ((err as Error).name !== 'AbortError') {
        setError((err as Error).message);
      }
    } finally {
      if (!controller.signal.aborted) {
        setIsLoading(false);
      }
    }
  }, []);

  useEffect(() => {
    void fetchSummary();
    return () => summaryAbortRef.current?.abort();
  }, [fetchSummary]);

  useEffect(() => {
    void fetchTransactions({
      ...filters,
      search: debouncedSearch,
    });
    return () => listAbortRef.current?.abort();
  }, [
    filters.category,
    filters.personalUse,
    filters.page,
    filters.pageSize,
    debouncedSearch,
    fetchTransactions,
  ]);

  const uploadFile = useCallback(
    async (file: File) => {
      setIsUploading(true);
      setError(null);
      setFeedback(null);

      try {
        const result = await statementApi.uploadStatement(file);
        setFeedback(result.message);
        await Promise.all([
          fetchSummary(),
          fetchTransactions({ ...filters, search: debouncedSearch, page: 1 }),
        ]);
        setFilters((prev) => ({ ...prev, page: 1 }));
      } catch (err) {
        setError((err as Error).message);
      } finally {
        setIsUploading(false);
      }
    },
    [debouncedSearch, fetchSummary, fetchTransactions, filters]
  );

  const deleteTransaction = useCallback(
    async (id: string) => {
      try {
        await statementApi.deleteTransaction(id);
        await Promise.all([
          fetchSummary(),
          fetchTransactions({ ...filters, search: debouncedSearch }),
        ]);
      } catch (err) {
        setError((err as Error).message);
      }
    },
    [debouncedSearch, fetchSummary, fetchTransactions, filters]
  );

  const updateFilter = useCallback(
    <K extends keyof TransactionFilters>(key: K, value: TransactionFilters[K]) => {
      setFilters((prev) => ({
        ...prev,
        [key]: value,
        page: key === 'page' ? (value as number) : 1,
      }));
    },
    []
  );

  return {
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
    clearMessages: useCallback(() => {
      setError(null);
      setFeedback(null);
    }, []),
  };
}
