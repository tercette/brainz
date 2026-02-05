import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useEvents } from '../hooks/useEvents';
import { usersApi } from '../api/usersApi';
import type { User } from '../types/user';
import Spinner from '../components/ui/Spinner';
import Pagination from '../components/ui/Pagination';
import EventList from '../components/events/EventList';

export default function UserEventsPage() {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [user, setUser] = useState<User | null>(null);
  const pageSize = 20;

  const { data, loading, error } = useEvents(userId!, page, pageSize);

  useEffect(() => {
    if (userId) {
      usersApi.getUser(userId).then((res) => setUser(res.data)).catch(() => {});
    }
  }, [userId]);

  return (
    <div>
      <button
        onClick={() => navigate('/users')}
        className="text-sm text-indigo-600 hover:text-indigo-800 mb-4 inline-block"
      >
        &larr; Voltar para usuários
      </button>

      {user && (
        <div className="bg-white rounded-lg shadow p-6 mb-6">
          <div className="flex items-center gap-4">
            <div className="h-12 w-12 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-700 font-bold text-lg">
              {user.displayName.charAt(0).toUpperCase()}
            </div>
            <div>
              <h1 className="text-xl font-bold text-gray-800">{user.displayName}</h1>
              <p className="text-sm text-gray-500">{user.email}</p>
              {user.department && (
                <p className="text-xs text-gray-400">{user.department} {user.jobTitle ? `\u2022 ${user.jobTitle}` : ''}</p>
              )}
            </div>
          </div>
        </div>
      )}

      <h2 className="text-lg font-semibold text-gray-800 mb-4">Eventos</h2>

      {loading && <Spinner />}
      {error && <p className="text-red-500 text-sm">{error}</p>}

      {data && !loading && (
        <>
          <p className="text-sm text-gray-500 mb-3">
            {data.totalCount} evento(s) encontrado(s)
          </p>
          <EventList events={data.items} />
          <Pagination page={data.page} totalPages={data.totalPages} onPageChange={setPage} />
        </>
      )}
    </div>
  );
}
