function toUtcDate(dateStr: string): Date {
  const str = dateStr.endsWith('Z') ? dateStr : dateStr + 'Z';
  return new Date(str);
}

export function formatDateTime(dateStr: string): string {
  return toUtcDate(dateStr).toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function formatDate(dateStr: string): string {
  return toUtcDate(dateStr).toLocaleDateString('pt-BR');
}

export function formatTime(dateStr: string): string {
  return toUtcDate(dateStr).toLocaleTimeString('pt-BR', {
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function timeAgo(dateStr: string): string {
  const diff = Date.now() - toUtcDate(dateStr).getTime();
  const minutes = Math.floor(diff / 60000);
  if (minutes < 1) return 'agora';
  if (minutes < 60) return `${minutes}min atrás`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `${hours}h atrás`;
  const days = Math.floor(hours / 24);
  return `${days}d atrás`;
}
