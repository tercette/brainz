import api from './axiosInstance';
import type { SyncLog } from '../types/common';

export const syncApi = {
  getLogs: () => api.get<SyncLog[]>('/sync/logs'),
  trigger: (type: 'Users' | 'Events' | 'Full') =>
    api.post('/sync/trigger', null, { params: { type } }),
};
