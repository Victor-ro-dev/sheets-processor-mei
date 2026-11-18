import type {
  ApiResponse,
  PagedResult,
  StatementSummary,
  Transaction,
  TransactionFilters,
  UploadResult,
} from '../types/statement';

const API_BASE_URL = (import.meta.env.VITE_API_URL as string | undefined)?.replace(/\/+$/, '') || 'http://localhost:5000';

async function parseApiResponse<T>(response: Response): Promise<T> {
  const payload = (await response.json()) as ApiResponse<T>;

  if (!response.ok || !payload.success || payload.data === null) {
    throw new Error(payload.message || `Erro na requisição (${response.status})`);
  }

  return payload.data;
}

export const statementApi = {
  async uploadStatement(file: File): Promise<{ data: UploadResult; message: string }> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${API_BASE_URL}/api/statement/upload`, {
      method: 'POST',
      body: formData,
    });

    const payload = (await response.json()) as ApiResponse<UploadResult>;
    if (!response.ok || !payload.success || payload.data === null) {
      throw new Error(payload.message || 'Falha ao processar extrato.');
    }

    return {
      data: payload.data,
      message: payload.message || 'Extrato processado com sucesso.',
    };
  },

  async getTransactions(
    filters: TransactionFilters,
    signal?: AbortSignal
  ): Promise<PagedResult<Transaction>> {
    const params = new URLSearchParams({
      page: String(filters.page),
      pageSize: String(filters.pageSize),
    });

    if (filters.category) params.set('category', filters.category);
    if (filters.personalUse !== 'all') params.set('personalUse', filters.personalUse);
    if (filters.search.trim()) params.set('search', filters.search.trim());

    const response = await fetch(
      `${API_BASE_URL}/api/statement/transactions?${params.toString()}`,
      { signal }
    );

    return parseApiResponse<PagedResult<Transaction>>(response);
  },

  async getSummary(signal?: AbortSignal): Promise<StatementSummary> {
    const response = await fetch(`${API_BASE_URL}/api/statement/summary`, { signal });
    return parseApiResponse<StatementSummary>(response);
  },

  async deleteTransaction(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/api/statement/transactions/${id}`, {
      method: 'DELETE',
    });
    await parseApiResponse<string>(response);
  },
};
