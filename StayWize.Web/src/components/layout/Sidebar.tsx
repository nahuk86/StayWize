import { NavLink } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

interface Props {
  isOpen: boolean;
  onClose: () => void;
}

interface NavItem {
  to: string;
  label: string;
  icon: string;
  roles?: string[];
}

const navItems: NavItem[] = [
  { to: '/dashboard', label: 'Dashboard', icon: 'DB' },
  { to: '/properties', label: 'Propiedades', icon: 'PR', roles: ['Admin', 'Owner'] },
  { to: '/clients', label: 'Clientes', icon: 'CL', roles: ['Admin', 'Owner'] },
  { to: '/reservations', label: 'Reservas', icon: 'RS', roles: ['Admin', 'Owner', 'HostLocal'] },
  { to: '/access-codes', label: 'Códigos de acceso', icon: 'AC', roles: ['Admin', 'Owner', 'HostLocal'] },
];

export function Sidebar({ isOpen, onClose }: Props) {
  const { user } = useAuth();

  const visibleItems = navItems.filter(
    (item) => !item.roles || (user?.role && item.roles.includes(user.role))
  );

  return (
    <aside
      className={`
        fixed top-0 left-0 h-full w-64 bg-white border-r border-gray-200 z-30 shadow-lg
        transform transition-transform duration-200 ease-in-out
        ${isOpen ? 'translate-x-0' : '-translate-x-full'}
        lg:static lg:translate-x-0 lg:shadow-none lg:z-auto
      `}
    >
      {/* Header sidebar mobile */}
      <div className="flex items-center justify-between px-4 py-3 border-b border-gray-200 lg:hidden">
        <span className="text-lg font-bold text-slate-800">StayWize</span>
        <button
          onClick={onClose}
          className="p-2 rounded-md text-gray-500 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500"
          aria-label="Cerrar menú"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      {/* Nav items */}
      <nav className="p-3 space-y-1 mt-2">
        {visibleItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            onClick={onClose}
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-md text-sm font-medium transition-colors
              focus:outline-none focus:ring-2 focus:ring-blue-500
              ${isActive
                ? 'bg-blue-50 text-blue-700'
                : 'text-slate-600 hover:bg-gray-100 hover:text-slate-800'
              }`
            }
          >
            <span
              className="text-xs font-bold bg-gray-100 text-gray-600 w-7 h-7 rounded flex items-center justify-center flex-shrink-0"
              aria-hidden="true"
            >
              {item.icon}
            </span>
            {item.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}