import { useState, useEffect, useCallback } from 'react';
import { usersApi } from '../api/usersApi';
import type { User } from '../types/user';
import type { PagedResult } from '../types/common';

export function useUsers(page: number, pageSize: number, search: string) {
  const [data, setData] = useState<PagedResult<User> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchUsers = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await usersApi.getUsers(page, pageSize, search || undefined);
      setData(response.data);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Falha ao buscar usu\u00e1rios';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, search]);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  return { data, loading, error, refetch: fetchUsers };
}
