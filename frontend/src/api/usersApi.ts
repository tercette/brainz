import api from './axiosInstance';
import type { User } from '../types/user';
import type { PagedResult } from '../types/common';

export const usersApi = {
  getUsers: (page: number, pageSize: number, search?: string) =>
    api.get<PagedResult<User>>('/users', { params: { page, pageSize, search } }),

  getUser: (id: string) => api.get<User>(`/users/${id}`),
};
