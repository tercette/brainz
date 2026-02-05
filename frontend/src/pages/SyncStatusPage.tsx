import { useState, useEffect, useCallback } from 'react';
import { syncApi } from '../api/syncApi';
import type { SyncLog } from '../types/common';
import { formatDateTime } from '../utils/dateUtils';
import Spinner from '../components/ui/Spinner';

export default function SyncStatusPage() {
  const [logs, setLogs] = useState<SyncLog[]>([]);
  const [loading, setLoading] = useState(true);
  const [triggering, setTriggering] = useState(false);

  const fetchLogs = useCallback(async () => {
    try {
      const res = await syncApi.getLogs();
      setLogs(res.data);
    } catch {
      // silently fail
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchLogs();
  }, [fetchLogs]);

  const handleTrigger = async (type: 'Users' | 'Events' | 'Full') => {
    setTriggering(true);
    try {
      await syncApi.trigger(type);
      setTimeout(fetchLogs, 2000);
    } catch {
      // silently fail
    } finally {
      setTriggering(false);
    }
  };

  const statusColor = (status: string) => {
    switch (status) {
      case 'Completed': return 'bg-green-100 text-green-800';
      case 'Running': return 'bg-blue-100 text-blue-800';
      case 'Failed': return 'bg-red-100 text-red-800';
      case 'CompletedWithErrors': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div>
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Sincronização</h1>
        <div className="flex gap-2">
          <button
            onClick={() => handleTrigger('Users')}
            disabled={triggering}
            className="px-3 py-1.5 text-sm bg-indigo-600 text-white rounded hover:bg-indigo-700 disabled:opacity-50 transition-colors"
          >
            Sync Usuários
          </button>
          <button
            onClick={() => handleTrigger('Events')}
            disabled={triggering}
            className="px-3 py-1.5 text-sm bg-indigo-600 text-white rounded hover:bg-indigo-700 disabled:opacity-50 transition-colors"
          >
            Sync Eventos
          </button>
          <button
            onClick={() => handleTrigger('Full')}
            disabled={triggering}
            className="px-3 py-1.5 text-sm bg-indigo-800 text-white rounded hover:bg-indigo-900 disabled:opacity-50 transition-colors"
          >
            Sync Completo
          </button>
        </div>
      </div>

      {loading ? (
        <Spinner />
      ) : logs.length === 0 ? (
        <p className="text-gray-500 text-sm">Nenhum log de sincronização encontrado.</p>
      ) : (
        <div className="overflow-x-auto bg-white rounded-lg shadow">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tipo</th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase hidden sm:table-cell">Início</th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase hidden md:table-cell">Fim</th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Registros</th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase hidden lg:table-cell">Erros</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {logs.map((log) => (
                <tr key={log.id}>
                  <td className="px-4 py-3 text-sm text-gray-900">{log.syncType}</td>
                  <td className="px-4 py-3">
                    <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${statusColor(log.status)}`}>
                      {log.status}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-xs text-gray-500 hidden sm:table-cell">
                    {formatDateTime(log.startedAt)}
                  </td>
                  <td className="px-4 py-3 text-xs text-gray-500 hidden md:table-cell">
                    {log.completedAt ? formatDateTime(log.completedAt) : '\u2014'}
                  </td>
                  <td className="px-4 py-3 text-sm text-gray-900">{log.recordsProcessed}</td>
                  <td className="px-4 py-3 text-sm text-red-500 hidden lg:table-cell">{log.recordsFailed}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
