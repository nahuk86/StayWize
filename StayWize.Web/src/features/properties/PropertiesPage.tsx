import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { propertyService, type PropertyDto } from './propertyService';
import { PropertyFormModal } from './PropertyFormModal';

export function PropertiesPage() {
  const queryClient = useQueryClient();
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<PropertyDto | null>(null);

  const { data, isLoading, isError } = useQuery({
    queryKey: ['properties'],
    queryFn: propertyService.getAll,
  });

  const deleteMutation = useMutation({
    mutationFn: propertyService.remove,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['properties'] }),
  });

  const handleDelete = (id: string) => {
    if (confirm('¿Estás seguro de eliminar esta propiedad?')) {
      deleteMutation.mutate(id);
    }
  };

  const handleEdit = (property: PropertyDto) => {
    setEditing(property);
    setShowModal(true);
  };

  const handleClose = () => {
    setEditing(null);
    setShowModal(false);
  };

  if (isLoading) return <div className="text-gray-500">Cargando...</div>;
  if (isError) return <div className="text-red-600">Error al cargar propiedades.</div>;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Propiedades</h1>
        <button
          onClick={() => setShowModal(true)}
          className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 font-medium"
        >
          + Nueva propiedad
        </button>
      </div>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-600 uppercase text-xs">
            <tr>
              <th className="px-4 py-3 text-left">Nombre</th>
              <th className="px-4 py-3 text-left">Ciudad</th>
              <th className="px-4 py-3 text-left">País</th>
              <th className="px-4 py-3 text-left">Huéspedes máx.</th>
              <th className="px-4 py-3 text-left">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {data?.map((property) => (
              <tr key={property.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-800">{property.name}</td>
                <td className="px-4 py-3 text-gray-600">{property.city}</td>
                <td className="px-4 py-3 text-gray-600">{property.country}</td>
                <td className="px-4 py-3 text-gray-600">{property.maxGuests}</td>
                <td className="px-4 py-3 flex gap-2">
                  <button
                    onClick={() => handleEdit(property)}
                    className="text-blue-600 hover:underline text-sm"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleDelete(property.id)}
                    className="text-red-600 hover:underline text-sm"
                  >
                    Eliminar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {data?.length === 0 && (
          <div className="text-center py-8 text-gray-400">
            No hay propiedades registradas.
          </div>
        )}
      </div>

      {showModal && (
        <PropertyFormModal
          property={editing}
          onClose={handleClose}
        />
      )}
    </div>
  );
}