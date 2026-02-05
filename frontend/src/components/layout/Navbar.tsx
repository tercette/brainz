import { Link } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

export default function Navbar() {
  const { username, logout } = useAuth();

  return (
    <header className="bg-indigo-700 text-white shadow-md">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-14">
          <Link to="/dashboard" className="text-lg font-bold tracking-tight">
            Brainz
          </Link>

          <nav className="hidden sm:flex items-center gap-6">
            <Link to="/dashboard" className="text-sm hover:text-indigo-200 transition-colors">
              Dashboard
            </Link>
            <Link to="/users" className="text-sm hover:text-indigo-200 transition-colors">
              Usuários
            </Link>
            <Link to="/sync" className="text-sm hover:text-indigo-200 transition-colors">
              Sincronização
            </Link>
          </nav>

          <div className="flex items-center gap-4">
            <span className="text-sm text-indigo-200 hidden sm:inline">{username}</span>
            <button
              onClick={logout}
              className="text-sm bg-indigo-800 hover:bg-indigo-900 px-3 py-1 rounded transition-colors"
            >
              Sair
            </button>
          </div>
        </div>
      </div>

      {/* Mobile nav */}
      <div className="sm:hidden border-t border-indigo-600">
        <div className="flex">
          <Link to="/dashboard" className="flex-1 text-center py-2 text-sm hover:bg-indigo-600 transition-colors">
            Home
          </Link>
          <Link to="/users" className="flex-1 text-center py-2 text-sm hover:bg-indigo-600 transition-colors">
            Usuários
          </Link>
          <Link to="/sync" className="flex-1 text-center py-2 text-sm hover:bg-indigo-600 transition-colors">
            Sync
          </Link>
        </div>
      </div>
    </header>
  );
}
