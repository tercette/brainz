export interface User {
  id: string;
  displayName: string;
  email: string;
  givenName?: string;
  surname?: string;
  department?: string;
  jobTitle?: string;
  isActive: boolean;
  lastSyncedAt?: string;
}
