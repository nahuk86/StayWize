import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { reservationService, type ReservationStatus } from './reservationService';
import { ReservationFormModal } from './ReservationFormModal';

const statusLabel: Record<ReservationStatus, string> = {
  Pending: 'Pendiente',
  Confirmed: 'Confirmada',
  Cancelled: 'Cancelada',
};

const statusColor: Record<ReservationStatus, string> = {
  Pending: 'bg-amber-50 text-amber-700 border-amber-200',
  Confirmed: 'bg-emerald-50 text-emerald-700 border-emerald-200',
  Cancelled: 'bg-red-50 text-red-600 border-red-200',
};

function StatusBadge({ status }: { status: ReservationStatus }) {
  return (
    <span className={`text-xs font-medium px-2 py-0.5 rounded-full border ${statusColor[status]}`}>
      {statusLabel[status]}
    </span>
  );
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString('es-AR', {
    day: '2-digit', month: '2-digit', year: 'numeric',
  });
}

export function ReservationsPage() {
  const queryClient = useQueryClient();
  const [showModal, setShowModal] = useState(false);

  const { data, isLoading, isError } = useQuery({
    queryKey: ['reservations'],
    queryFn: reservationService.getAll,
  });

  const confirmMutation = useMutation({
    mutationFn: reservationService.confirm,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['reservations'] }),
  });

  const cancelMutation = useMutation({
    mutationFn: reservationService.cancel,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['reservations'] }),
  });

  const handleConfirm = (id: string) => {
    if (confirm('¿Confirmar esta reserva?')) confirmMutation.mutate(id);
  };

  const handleCancel = (id: string) => {
    if (confirm('¿Cancelar esta reserva?')) cancelMutation.mutate(id);
  };

  if (isLoading) return <div className="text-gray-400 text-sm">Cargando...</div>;
  if (isError) return <div className="text-red-600 text-sm">Error al cargar reservas.</div>;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-xl font-bold text-slate-800">Reservas</h1>
          <p className="text-sm text-gray-500">Gestión de reservas</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm font-medium"
        >
          + Nueva reserva
        </button>
      </div>

      {/* Tabla — desktop */}
      <div className="hidden md:block bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs border-b border-gray-200">
            <tr>
              <th className="px-4 py-3 text-left">Propiedad</th>
              <th className="px-4 py-3 text-left">Cliente</th>
              <th className="px-4 py-3 text-left">Check-in</th>
              <th className="px-4 py-3 text-left">Check-out</th>
              <th className="px-4 py-3 text-left">Huéspedes</th>
              <th className="px-4 py-3 text-left">Estado</th>
              <th className="px-4 py-3 text-left">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {data?.map((r) => (
              <tr key={r.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-slate-800">{r.propertyName}</td>
                <td className="px-4 py-3 text-gray-600">{r.clientName}</td>
                <td className="px-4 py-3 text-gray-600">{formatDate(r.checkIn)}</td>
                <td className="px-4 py-3 text-gray-600">{formatDate(r.checkOut)}</td>
                <td className="px-4 py-3 text-gray-600">{r.guestCount}</td>
                <td className="px-4 py-3"><StatusBadge status={r.status} /></td>
                <td className="px-4 py-3">
                  <div className="flex gap-2">
                    {r.status === 'Pending' && (
                      <button onClick={() => handleConfirm(r.id)} className="text-emerald-600 hover:underline text-sm">Confirmar</button>
                    )}
                    {r.status !== 'Cancelled' && (
                      <button onClick={() => handleCancel(r.id)} className="text-red-600 hover:underline text-sm">Cancelar</button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {data?.length === 0 && (
          <div className="text-center py-10 text-gray-400 text-sm">No hay reservas registradas.</div>
        )}
      </div>

      {/* Cards — mobile */}
      <div className="md:hidden space-y-3">
        {data?.length === 0 && (
          <div className="text-center py-10 text-gray-400 text-sm">No hay reservas registradas.</div>
        )}
        {data?.map((r) => (
          <div key={r.id} className="bg-white rounded-lg border border-gray-200 p-4 shadow-sm">
            <div className="flex items-start justify-between mb-2">
              <span className="font-semibold text-slate-800">{r.propertyName}</span>
              <StatusBadge status={r.status} />
            </div>
            <div className="space-y-1 text-sm text-gray-600">
              <p>{r.clientName}</p>
              <p>{formatDate(r.checkIn)} → {formatDate(r.checkOut)}</p>
              <p className="text-xs text-gray-400">{r.guestCount} huéspedes</p>
            </div>
            <div className="flex gap-3 mt-3 pt-3 border-t border-gray-100">
              {r.status === 'Pending' && (
                <button onClick={() => handleConfirm(r.id)} className="text-emerald-600 text-sm font-medium">Confirmar</button>
              )}
              {r.status !== 'Cancelled' && (
                <button onClick={() => handleCancel(r.id)} className="text-red-600 text-sm font-medium">Cancelar</button>
              )}
            </div>
          </div>
        ))}
      </div>

      {showModal && <ReservationFormModal onClose={() => setShowModal(false)} />}
    </div>
  );
}