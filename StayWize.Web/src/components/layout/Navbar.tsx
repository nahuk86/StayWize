import { useAuth } from '../../hooks/useAuth';
import { useNavigate } from 'react-router-dom';

export function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-blue-700 text-white px-6 py-4 flex items-center justify-between shadow-md">
      <span className="text-xl font-bold tracking-wide">StayWize</span>
      <div className="flex items-center gap-4">
        <span className="text-sm text-blue-100">{user?.email}</span>
        <span className="text-xs bg-blue-500 px-2 py-1 rounded-full">{user?.role}</span>
        <button
          onClick={handleLogout}
          className="text-sm bg-white text-blue-700 px-3 py-1 rounded-md font-medium hover:bg-blue-50"
        >
          Cerrar sesión
        </button>
      </div>
    </nav>
  );
}