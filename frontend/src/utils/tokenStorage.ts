const TOKEN_KEY = 'brainz_token';
const USERNAME_KEY = 'brainz_username';

export const getToken = (): string | null => localStorage.getItem(TOKEN_KEY);
export const setToken = (token: string): void => localStorage.setItem(TOKEN_KEY, token);
export const removeToken = (): void => {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USERNAME_KEY);
};

export const getUsername = (): string | null => localStorage.getItem(USERNAME_KEY);
export const setUsername = (username: string): void => localStorage.setItem(USERNAME_KEY, username);
