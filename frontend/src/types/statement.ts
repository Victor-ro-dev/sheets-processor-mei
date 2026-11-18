export interface ApiResponse<T> {
  success: boolean;
  message: string | null;
  data: T | null;
  errors: string[] | null;
}

export interface Transaction {
  id: string;
  date: string;
  value: number;
  description: string;
  hash: string;
  category: string;
  personalUse: boolean;
}

export interface CategoryBreakdown {
  category: string;
  count: number;
  totalValue: number;
}

export interface StatementSummary {
  totalCount: number;
  totalRevenue: number;
  totalExpenses: number;
  netBalance: number;
  taxPaid: number;
  personalExpenses: number;
  categories: CategoryBreakdown[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface UploadResult {
  insertedCount: number;
  skippedDuplicatesCount: number;
  transactions: Transaction[];
}

export interface TransactionFilters {
  category: string;
  personalUse: 'all' | 'true' | 'false';
  search: string;
  page: number;
  pageSize: number;
}
