import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { usersApi } from '../api/usersApi';
import { syncApi } from '../api/syncApi';
import type { SyncLog } from '../types/common';
import { formatDateTime, timeAgo } from '../utils/dateUtils';
import Spinner from '../components/ui/Spinner';

interface DashboardStats {
  totalUsers: number;
  lastSync: SyncLog | null;
  lastUserSync: SyncLog | null;
  lastEventSync: SyncLog | null;
}

function parseProgress(details?: string): number | null {
  if (!details) return null;
  const match = details.match(/^(\d+)\/(\d+)/);
  if (!match) return null;
  const [, current, total] = match;
  const t = parseInt(total);
  if (t === 0) return 0;
  return Math.round((parseInt(current) / t) * 100);
}

function ProgressBar({ percent }: { percent: number }) {
  return (
    <div className="w-full bg-gray-200 rounded-full h-2 mt-2">
      <div
        className="bg-indigo-500 h-2 rounded-full transition-all duration-500"
        style={{ width: `${Math.min(percent, 100)}%` }}
      />
    </div>
  );
}

export default function DashboardPage() {
  const navigate = useNavigate();
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchStats() {
      try {
        const [usersRes, logsRes] = await Promise.all([
          usersApi.getUsers(1, 1),
          syncApi.getLogs(),
        ]);

        const logs = logsRes.data;

        const findLastSync = (type: string) => {
          const running = logs.find((l: SyncLog) => l.syncType === type && l.status === 'Running');
          if (running) return running;
          return logs.find((l: SyncLog) => l.syncType === type && (l.status === 'Completed' || l.status === 'CompletedWithErrors')) || null;
        };

        const lastSync = logs.find((l: SyncLog) => l.status === 'Completed' || l.status === 'CompletedWithErrors')
          || logs[0] || null;

        setStats({
          totalUsers: usersRes.data.totalCount,
          lastSync,
          lastUserSync: findLastSync('Users'),
          lastEventSync: findLastSync('Events'),
        });
      } catch {
        // silently fail
      } finally {
        setLoading(false);
      }
    }
    fetchStats();
    const interval = setInterval(fetchStats, 10000);
    return () => clearInterval(interval);
  }, []);

  if (loading) return <Spinner />;

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800 mb-6">Dashboard</h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
        <div
          onClick={() => navigate('/users')}
          className="bg-white rounded-lg shadow p-6 cursor-pointer hover:shadow-md transition-shadow"
        >
          <p className="text-sm text-gray-500 mb-1">Total de Usuários</p>
          <p className="text-3xl font-bold text-indigo-700">
            {stats?.totalUsers?.toLocaleString('pt-BR') ?? '—'}
          </p>
        </div>

        <div className="bg-white rounded-lg shadow p-6">
          <p className="text-sm text-gray-500 mb-1">Sync de Usuários</p>
          {stats?.lastUserSync ? (
            stats.lastUserSync.status === 'Running' ? (
              (() => {
                const percent = parseProgress(stats.lastUserSync.details);
                return (
                  <>
                    <p className="text-lg font-semibold text-blue-600">
                      {stats.lastUserSync.details || 'Em andamento...'}
                    </p>
                    {percent !== null && (
                      <>
                        <ProgressBar percent={percent} />
                        <p className="text-xs text-gray-500 mt-1">{percent}%</p>
                      </>
                    )}
                    <p className="text-xs text-gray-400 mt-1">
                      Iniciado {timeAgo(stats.lastUserSync.startedAt)}
                    </p>
                  </>
                );
              })()
            ) : (
              <>
                <p className="text-lg font-semibold text-gray-800">
                  {stats.lastUserSync.recordsProcessed.toLocaleString('pt-BR')} registros
                </p>
                <p className="text-xs text-gray-400">
                  {stats.lastUserSync.completedAt
                    ? `${formatDateTime(stats.lastUserSync.completedAt)} (${timeAgo(stats.lastUserSync.completedAt)})`
                    : '—'}
                </p>
              </>
            )
          ) : (
            <p className="text-lg text-gray-400">Nenhum sync realizado</p>
          )}
        </div>

        <div className="bg-white rounded-lg shadow p-6">
          <p className="text-sm text-gray-500 mb-1">Sync de Eventos</p>
          {stats?.lastEventSync ? (
            stats.lastEventSync.status === 'Running' ? (
              (() => {
                const percent = parseProgress(stats.lastEventSync.details);
                return (
                  <>
                    <p className="text-lg font-semibold text-blue-600">
                      {stats.lastEventSync.details || 'Em andamento...'}
                    </p>
                    {percent !== null && (
                      <>
                        <ProgressBar percent={percent} />
                        <p className="text-xs text-gray-500 mt-1">{percent}%</p>
                      </>
                    )}
                    <p className="text-xs text-gray-400 mt-1">
                      Iniciado {timeAgo(stats.lastEventSync.startedAt)}
                    </p>
                  </>
                );
              })()
            ) : (
              <>
                <p className="text-lg font-semibold text-gray-800">
                  {stats.lastEventSync.recordsProcessed.toLocaleString('pt-BR')} registros
                </p>
                <p className="text-xs text-gray-400">
                  {stats.lastEventSync.completedAt
                    ? `${formatDateTime(stats.lastEventSync.completedAt)} (${timeAgo(stats.lastEventSync.completedAt)})`
                    : '—'}
                </p>
              </>
            )
          ) : (
            <p className="text-lg text-gray-400">Nenhum sync realizado</p>
          )}
        </div>
      </div>

      {stats?.lastSync && (
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex justify-between items-center mb-3">
            <h2 className="text-lg font-semibold text-gray-800">Última Sincronização</h2>
            <button
              onClick={() => navigate('/sync')}
              className="text-sm text-indigo-600 hover:text-indigo-800"
            >
              Ver todos os logs &rarr;
            </button>
          </div>
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 text-sm">
            <div>
              <p className="text-gray-500">Tipo</p>
              <p className="font-medium">{stats.lastSync.syncType}</p>
            </div>
            <div>
              <p className="text-gray-500">Status</p>
              <p className="font-medium">{stats.lastSync.status}</p>
            </div>
            <div>
              <p className="text-gray-500">Registros</p>
              <p className="font-medium">{stats.lastSync.recordsProcessed.toLocaleString('pt-BR')}</p>
            </div>
            <div>
              <p className="text-gray-500">Erros</p>
              <p className="font-medium">{stats.lastSync.recordsFailed}</p>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
