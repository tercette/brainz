import api from './axiosInstance';
import type { LoginRequest, LoginResponse } from '../types/auth';

export const authApi = {
  login: (data: LoginRequest) => api.post<LoginResponse>('/auth/login', data),
};
