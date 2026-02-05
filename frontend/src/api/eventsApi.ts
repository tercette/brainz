import api from './axiosInstance';
import type { CalendarEvent } from '../types/event';
import type { PagedResult } from '../types/common';

export const eventsApi = {
  getUserEvents: (
    userId: string,
    page: number,
    pageSize: number,
    from?: string,
    to?: string
  ) =>
    api.get<PagedResult<CalendarEvent>>(`/users/${userId}/events`, {
      params: { page, pageSize, from, to },
    }),
};
