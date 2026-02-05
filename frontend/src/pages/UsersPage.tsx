import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useUsers } from '../hooks/useUsers';
import { useDebounce } from '../hooks/useDebounce';
import SearchInput from '../components/ui/SearchInput';
import Pagination from '../components/ui/Pagination';
import Spinner from '../components/ui/Spinner';
import UserTable from '../components/users/UserTable';

export default function UsersPage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const debouncedSearch = useDebounce(search, 400);
  const pageSize = 20;

  const { data, loading, error } = useUsers(page, pageSize, debouncedSearch);

  const handleUserClick = (userId: string) => {
    navigate(`/users/${userId}/events`);
  };

  const handleSearchChange = (value: string) => {
    setSearch(value);
    setPage(1);
  };

  return (
    <div>
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Usu\u00e1rios</h1>
        <SearchInput
          value={search}
          onChange={handleSearchChange}
          placeholder="Buscar por nome, email ou departamento..."
        />
      </div>

      {loading && <Spinner />}
      {error && <p className="text-red-500 text-sm">{error}</p>}

      {data && !loading && (
        <>
          <p className="text-sm text-gray-500 mb-3">
            {data.totalCount} usu\u00e1rio(s) encontrado(s)
          </p>
          <UserTable users={data.items} onUserClick={handleUserClick} />
          <Pagination page={data.page} totalPages={data.totalPages} onPageChange={setPage} />
        </>
      )}
    </div>
  );
}
