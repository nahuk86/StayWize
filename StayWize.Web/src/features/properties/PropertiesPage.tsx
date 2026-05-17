import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { propertyService, type PropertyDto } from './propertyService';
import { PropertyFormModal } from './PropertyFormModal';
import { AssignHostLocalModal } from './AssignHostLocalModal';

export function PropertiesPage() {
  const queryClient = useQueryClient();
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<PropertyDto | null>(null);
  const [assigning, setAssigning] = useState<PropertyDto | null>(null);

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

  if (isLoading) return <div className="text-gray-400 text-sm">Cargando...</div>;
  if (isError) return <div className="text-red-600 text-sm">Error al cargar propiedades.</div>;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-xl font-bold text-slate-800">Propiedades</h1>
          <p className="text-sm text-gray-500">Gestión de propiedades registradas</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm font-medium"
        >
          + Nueva propiedad
        </button>
      </div>

      {/* Tabla — desktop */}
      <div className="hidden md:block bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs border-b border-gray-200">
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
                <td className="px-4 py-3 font-medium text-slate-800">{property.name}</td>
                <td className="px-4 py-3 text-gray-600">{property.city}</td>
                <td className="px-4 py-3 text-gray-600">{property.country}</td>
                <td className="px-4 py-3 text-gray-600">{property.maxGuests}</td>
                <td className="px-4 py-3">
                  <div className="flex gap-3">
                    <button onClick={() => handleEdit(property)} className="text-blue-600 hover:underline text-sm">Editar</button>
                    <button onClick={() => setAssigning(property)} className="text-amber-600 hover:underline text-sm">Asignar HL</button>
                    <button onClick={() => handleDelete(property.id)} className="text-red-600 hover:underline text-sm">Eliminar</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {data?.length === 0 && (
          <div className="text-center py-10 text-gray-400 text-sm">No hay propiedades registradas.</div>
        )}
      </div>

      {/* Cards — mobile */}
      <div className="md:hidden space-y-3">
        {data?.length === 0 && (
          <div className="text-center py-10 text-gray-400 text-sm">No hay propiedades registradas.</div>
        )}
        {data?.map((property) => (
          <div key={property.id} className="bg-white rounded-lg border border-gray-200 p-4 shadow-sm">
            <div className="mb-2">
              <span className="font-semibold text-slate-800">{property.name}</span>
            </div>
            <div className="space-y-1 text-sm text-gray-600">
              <p>{property.city}, {property.country}</p>
              <p className="text-xs text-gray-400">Máx. {property.maxGuests} huéspedes</p>
            </div>
            <div className="flex gap-3 mt-3 pt-3 border-t border-gray-100">
              <button onClick={() => handleEdit(property)} className="text-blue-600 text-sm font-medium">Editar</button>
              <button onClick={() => setAssigning(property)} className="text-amber-600 text-sm font-medium">Asignar HL</button>
              <button onClick={() => handleDelete(property.id)} className="text-red-600 text-sm font-medium">Eliminar</button>
            </div>
          </div>
        ))}
      </div>

      {showModal && (
        <PropertyFormModal property={editing} onClose={handleClose} />
      )}

      {assigning && (
        <AssignHostLocalModal
          property={assigning}
          onClose={() => setAssigning(null)}
        />
      )}
    </div>
  );
}