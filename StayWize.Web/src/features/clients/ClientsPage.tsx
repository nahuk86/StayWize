import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { clientService, type ClientDto } from './clientService';
import { ClientFormModal } from './ClientFormModal';

export function ClientsPage() {
  const queryClient = useQueryClient();
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<ClientDto | null>(null);

  const { data, isLoading, isError } = useQuery({
    queryKey: ['clients'],
    queryFn: clientService.getAll,
  });

  const deleteMutation = useMutation({
    mutationFn: clientService.remove,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['clients'] }),
  });

  const handleDelete = (id: string) => {
    if (confirm('¿Estás seguro de eliminar este cliente?')) {
      deleteMutation.mutate(id);
    }
  };

  const handleEdit = (client: ClientDto) => {
    setEditing(client);
    setShowModal(true);
  };

  const handleClose = () => {
    setEditing(null);
    setShowModal(false);
  };

  if (isLoading) return <div className="text-gray-400 text-sm">Cargando...</div>;
  if (isError) return <div className="text-red-600 text-sm">Error al cargar clientes.</div>;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-xl font-bold text-slate-800">Clientes</h1>
          <p className="text-sm text-gray-500">Gestión de clientes registrados</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm font-medium"
        >
          + Nuevo cliente
        </button>
      </div>

      {/* Tabla — desktop */}
      <div className="hidden md:block bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs border-b border-gray-200">
            <tr>
              <th className="px-4 py-3 text-left">Nombre</th>
              <th className="px-4 py-3 text-left">Email</th>
              <th className="px-4 py-3 text-left">Teléfono</th>
              <th className="px-4 py-3 text-left">Documento</th>
              <th className="px-4 py-3 text-left">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {data?.map((client) => (
              <tr key={client.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-slate-800">
                  {client.firstName} {client.lastName}
                </td>
                <td className="px-4 py-3 text-gray-600">{client.email}</td>
                <td className="px-4 py-3 text-gray-600">{client.phone}</td>
                <td className="px-4 py-3 text-gray-600">{client.documentNumber}</td>
                <td className="px-4 py-3 flex gap-3">
                  <button onClick={() => handleEdit(client)} className="text-blue-600 hover:underline text-sm">Editar</button>
                  <button onClick={() => handleDelete(client.id)} className="text-red-600 hover:underline text-sm">Eliminar</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {data?.length === 0 && (
          <div className="text-center py-10 text-gray-400 text-sm">No hay clientes registrados.</div>
        )}
      </div>

      {/* Cards — mobile */}
      <div className="md:hidden space-y-3">
        {data?.length === 0 && (
          <div className="text-center py-10 text-gray-400 text-sm">No hay clientes registrados.</div>
        )}
        {data?.map((client) => (
          <div key={client.id} className="bg-white rounded-lg border border-gray-200 p-4 shadow-sm">
            <div className="flex items-start justify-between mb-2">
              <span className="font-semibold text-slate-800">
                {client.firstName} {client.lastName}
              </span>
            </div>
            <div className="space-y-1 text-sm text-gray-600">
              <p>{client.email}</p>
              <p>{client.phone}</p>
              <p className="text-xs text-gray-400">Doc: {client.documentNumber}</p>
            </div>
            <div className="flex gap-3 mt-3 pt-3 border-t border-gray-100">
              <button onClick={() => handleEdit(client)} className="text-blue-600 text-sm font-medium">Editar</button>
              <button onClick={() => handleDelete(client.id)} className="text-red-600 text-sm font-medium">Eliminar</button>
            </div>
          </div>
        ))}
      </div>

      {showModal && <ClientFormModal client={editing} onClose={handleClose} />}
    </div>
  );
}