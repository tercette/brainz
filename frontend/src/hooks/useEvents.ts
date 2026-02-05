import { useState, useEffect, useCallback } from 'react';
import { eventsApi } from '../api/eventsApi';
import type { CalendarEvent } from '../types/event';
import type { PagedResult } from '../types/common';

export function useEvents(
  userId: string,
  page: number,
  pageSize: number,
  from?: string,
  to?: string
) {
  const [data, setData] = useState<PagedResult<CalendarEvent> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchEvents = useCallback(async () => {
    if (!userId) return;
    setLoading(true);
    setError(null);
    try {
      const response = await eventsApi.getUserEvents(userId, page, pageSize, from, to);
      setData(response.data);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Falha ao buscar eventos';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [userId, page, pageSize, from, to]);

  useEffect(() => {
    fetchEvents();
  }, [fetchEvents]);

  return { data, loading, error, refetch: fetchEvents };
}
