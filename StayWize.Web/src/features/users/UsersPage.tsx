import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { userService, type UserDto, type UserRole } from './userService';
import { UserFormModal } from './UserFormModal';
import { useAuth } from '../../hooks/useAuth';

const roleColor: Record<UserRole, string> = {
  Admin: 'bg-purple-50 text-purple-700 border-purple-200',
  Owner: 'bg-blue-50 text-blue-700 border-blue-200',
  HostLocal: 'bg-amber-50 text-amber-700 border-amber-200',
  Guest: 'bg-gray-50 text-gray-600 border-gray-200',
};

function RoleBadge({ role }: { role: UserRole }) {
  return (
    <span className={`text-xs font-medium px-2 py-0.5 rounded-full border ${roleColor[role]}`}>
      {role}
    </span>
  );
}

export function UsersPage() {
  const queryClient = useQueryClient();
  const { user: currentUser } = useAuth();
  const isAdmin = currentUser?.role === 'Admin';
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<UserDto | null>(null);

  const { data, isLoading, isError } = useQuery({
    queryKey: ['users'],
    queryFn: userService.getAll,
  });

  const deleteMutation = useMutation({
    mutationFn: userService.remove,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }),
  });

  const handleEdit = (user: UserDto) => {
    setEditing(user);
    setShowModal(true);
  };

  const handleDelete = (id: string) => {
    if (confirm('¿Estás seguro de eliminar este usuario?')) {
      deleteMutation.mutate(id);
    }
  };

  const handleClose = () => {
    setEditing(null);
    setShowModal(false);
  };

  if (isLoading) return <div className="text-gray-400 text-sm">Cargando...</div>;
  if (isError) return <div className="text-red-600 text-sm">Error al cargar usuarios.</div>;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-xl font-bold text-slate-800">Usuarios</h1>
          <p className="text-sm text-gray-500">Gestión de usuarios del sistema</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm font-medium"
        >
          + Nuevo usuario
        </button>
      </div>

      {/* Tabla — desktop */}
      <div className="hidden md:block bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs border-b border-gray-200">
            <tr>
              <th className="px-4 py-3 text-left">Nombre</th>
              <th className="px-4 py-3 text-left">Email</th>
              <th className="px-4 py-3 text-left">Rol</th>
              <th className="px-4 py-3 text-left">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {data?.map((user) => (
              <tr key={user.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-slate-800">
                  {user.firstName} {user.lastName}
                </td>
                <td className="px-4 py-3 text-gray-600">{user.email}</td>
                <td className="px-4 py-3"><RoleBadge role={user.role} /></td>
                <td className="px-4 py-3">
                  <div className="flex gap-3">
                    <button
                      onClick={() => handleEdit(user)}
                      className="text-blue-600 hover:underline text-sm"
                    >
                      Editar
                    </button>
                    {isAdmin && (
                      <button
                        onClick={() => handleDelete(user.id)}
                        className="text-red-600 hover:underline text-sm"
                      >
                        Eliminar
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {data?.length === 0 && (
          <div className="text-center py-10 text-gray-400 text-sm">No hay usuarios registrados.</div>
        )}
      </div>

      {/* Cards — mobile */}
      <div className="md:hidden space-y-3">
        {data?.length === 0 && (
          <div className="text-center py-10 text-gray-400 text-sm">No hay usuarios registrados.</div>
        )}
        {data?.map((user) => (
          <div key={user.id} className="bg-white rounded-lg border border-gray-200 p-4 shadow-sm">
            <div className="flex items-start justify-between mb-2">
              <span className="font-semibold text-slate-800">
                {user.firstName} {user.lastName}
              </span>
              <RoleBadge role={user.role} />
            </div>
            <p className="text-sm text-gray-600">{user.email}</p>
            <div className="flex gap-3 mt-3 pt-3 border-t border-gray-100">
              <button
                onClick={() => handleEdit(user)}
                className="text-blue-600 text-sm font-medium"
              >
                Editar
              </button>
              {isAdmin && (
                <button
                  onClick={() => handleDelete(user.id)}
                  className="text-red-600 text-sm font-medium"
                >
                  Eliminar
                </button>
              )}
            </div>
          </div>
        ))}
      </div>

      {showModal && <UserFormModal user={editing} onClose={handleClose} />}
    </div>
  );
}