import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { userService } from '../users/userService';
import { propertyService, type PropertyDto } from './propertyService';

interface Props {
  property: PropertyDto;
  onClose: () => void;
}

export function AssignHostLocalModal({ property, onClose }: Props) {
  const queryClient = useQueryClient();
  const [selectedUserId, setSelectedUserId] = useState('');
  const [error, setError] = useState<string | null>(null);

  const { data: users } = useQuery({
    queryKey: ['users'],
    queryFn: userService.getAll,
  });

  const hostLocals = users?.filter((u) => u.role === 'HostLocal') ?? [];

  const assignMutation = useMutation({
    mutationFn: (userId: string) =>
      propertyService.assignHostLocal(property.id, userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['properties'] });
      onClose();
    },
    onError: (err: any) => {
      setError(err.response?.data?.message ?? 'Error al asignar el host local.');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedUserId) return;
    setError(null);
    assignMutation.mutate(selectedUserId);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-md p-6">
        <h2 className="text-lg font-bold text-slate-800 mb-1">
          Asignar Host Local
        </h2>
        <p className="text-sm text-gray-500 mb-4">
          Propiedad: <span className="font-medium text-slate-700">{property.name}</span>
        </p>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Seleccioná un Host Local
            </label>
            <select
              value={selectedUserId}
              onChange={(e) => setSelectedUserId(e.target.value)}
              className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">-- Seleccioná --</option>
              {hostLocals.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.firstName} {u.lastName} ({u.email})
                </option>
              ))}
            </select>
            {hostLocals.length === 0 && (
              <p className="text-amber-600 text-xs mt-1">
                No hay usuarios con rol HostLocal registrados.
              </p>
            )}
          </div>

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md text-sm">
              {error}
            </div>
          )}

          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 border border-gray-300 text-gray-700 py-2 rounded-md hover:bg-gray-50 text-sm"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={!selectedUserId || assignMutation.isPending}
              className="flex-1 bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 disabled:opacity-50 text-sm font-medium"
            >
              {assignMutation.isPending ? 'Asignando...' : 'Asignar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}