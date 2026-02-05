import { createContext, useState, useEffect, type ReactNode } from 'react';
import type { LoginRequest } from '../types/auth';
import { authApi } from '../api/authApi';
import { getToken, setToken, removeToken, setUsername, getUsername } from '../utils/tokenStorage';

interface AuthContextType {
  isAuthenticated: boolean;
  username: string | null;
  login: (data: LoginRequest) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}

export const AuthContext = createContext<AuthContextType>(null!);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [username, setUser] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const token = getToken();
    if (token) {
      setIsAuthenticated(true);
      setUser(getUsername());
    }
    setIsLoading(false);
  }, []);

  const login = async (data: LoginRequest) => {
    const response = await authApi.login(data);
    const result = response.data;
    setToken(result.token);
    setUsername(result.username);
    setIsAuthenticated(true);
    setUser(result.username);
  };

  const logout = () => {
    removeToken();
    setIsAuthenticated(false);
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, username, login, logout, isLoading }}>
      {children}
    </AuthContext.Provider>
  );
}
