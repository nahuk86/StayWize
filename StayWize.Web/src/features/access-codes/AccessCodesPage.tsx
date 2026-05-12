import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { accessCodeService, type AccessCodeStatus } from './accessCodeService';
import { GenerateAccessCodeModal } from './GenerateAccessCodeModal';
import { reservationService } from '../reservations/reservationService';

const statusLabel: Record<AccessCodeStatus, string> = {
  Active: 'Activo',
  Revoked: 'Revocado',
  Expired: 'Expirado',
};

const statusColor: Record<AccessCodeStatus, string> = {
  Active: 'bg-emerald-50 text-emerald-700 border-emerald-200',
  Revoked: 'bg-red-50 text-red-600 border-red-200',
  Expired: 'bg-gray-50 text-gray-500 border-gray-200',
};

function StatusBadge({ status }: { status: AccessCodeStatus }) {
  return (
    <span className={`text-xs font-medium px-2 py-0.5 rounded-full border ${statusColor[status]}`}>
      {statusLabel[status]}
    </span>
  );
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString('es-AR', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
  });
}

export function AccessCodesPage() {
  const queryClient = useQueryClient();
  const [searchParams] = useSearchParams();
  const [showModal, setShowModal] = useState(false);
  const [selectedReservationId, setSelectedReservationId] = useState<string>(
    searchParams.get('reservationId') ?? ''
  );

  const { data: reservations } = useQuery({
    queryKey: ['reservations'],
    queryFn: reservationService.getAll,
  });

  const { data: codes, isLoading } = useQuery({
    queryKey: ['access-codes', selectedReservationId],
    queryFn: () => accessCodeService.getByReservation(selectedReservationId),
    enabled: !!selectedReservationId,
  });

  const revokeMutation = useMutation({
    mutationFn: accessCodeService.revoke,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['access-codes', selectedReservationId] }),
  });

  const handleRevoke = (id: string) => {
    if (confirm('¿Revocar este código de acceso?')) revokeMutation.mutate(id);
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-xl font-bold text-slate-800">Códigos de acceso</h1>
          <p className="text-sm text-gray-500">Gestión de códigos por reserva</p>
        </div>
        {selectedReservationId && (
          <button
            onClick={() => setShowModal(true)}
            className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm font-medium"
          >
            + Generar código
          </button>
        )}
      </div>

      {/* Selector de reserva */}
      <div className="bg-white rounded-lg border border-gray-200 p-4 mb-4 shadow-sm">
        <label className="block text-sm font-medium text-gray-700 mb-1">Seleccioná una reserva</label>
        <select
          value={selectedReservationId}
          onChange={(e) => setSelectedReservationId(e.target.value)}
          className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="">-- Seleccioná una reserva --</option>
          {reservations?.map((r) => (
            <option key={r.id} value={r.id}>
              {r.propertyName} — {r.clientName} ({new Date(r.checkIn).toLocaleDateString('es-AR')})
            </option>
          ))}
        </select>
      </div>

      {/* Tabla — desktop */}
      {selectedReservationId && (
        <>
          {isLoading && <div className="text-gray-400 text-sm">Cargando...</div>}

          <div className="hidden md:block bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 text-gray-500 uppercase text-xs border-b border-gray-200">
                <tr>
                  <th className="px-4 py-3 text-left">Código</th>
                  <th className="px-4 py-3 text-left">Tipo</th>
                  <th className="px-4 py-3 text-left">Válido desde</th>
                  <th className="px-4 py-3 text-left">Válido hasta</th>
                  <th className="px-4 py-3 text-left">Estado</th>
                  <th className="px-4 py-3 text-left">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {codes?.map((code) => (
                  <tr key={code.id} className="hover:bg-gray-50">
                    <td className="px-4 py-3 font-mono font-bold text-slate-800">{code.code}</td>
                    <td className="px-4 py-3 text-gray-600">{code.type}</td>
                    <td className="px-4 py-3 text-gray-600">{formatDate(code.validFrom)}</td>
                    <td className="px-4 py-3 text-gray-600">{formatDate(code.validTo)}</td>
                    <td className="px-4 py-3"><StatusBadge status={code.status} /></td>
                    <td className="px-4 py-3">
                      {code.status === 'Active' && (
                        <button
                          onClick={() => handleRevoke(code.id)}
                          className="text-red-600 hover:underline text-sm"
                        >
                          Revocar
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {codes?.length === 0 && (
              <div className="text-center py-10 text-gray-400 text-sm">
                No hay códigos para esta reserva.
              </div>
            )}
          </div>

          {/* Cards — mobile */}
          <div className="md:hidden space-y-3">
            {codes?.map((code) => (
              <div key={code.id} className="bg-white rounded-lg border border-gray-200 p-4 shadow-sm">
                <div className="flex items-start justify-between mb-2">
                  <span className="font-mono font-bold text-slate-800 text-lg">{code.code}</span>
                  <StatusBadge status={code.status} />
                </div>
                <div className="space-y-1 text-sm text-gray-600">
                  <p>Tipo: {code.type}</p>
                  <p>Desde: {formatDate(code.validFrom)}</p>
                  <p>Hasta: {formatDate(code.validTo)}</p>
                </div>
                {code.status === 'Active' && (
                  <div className="mt-3 pt-3 border-t border-gray-100">
                    <button
                      onClick={() => handleRevoke(code.id)}
                      className="text-red-600 text-sm font-medium"
                    >
                      Revocar
                    </button>
                  </div>
                )}
              </div>
            ))}
            {codes?.length === 0 && (
              <div className="text-center py-10 text-gray-400 text-sm">
                No hay códigos para esta reserva.
              </div>
            )}
          </div>
        </>
      )}

      {showModal && selectedReservationId && (
        <GenerateAccessCodeModal
          reservationId={selectedReservationId}
          onClose={() => setShowModal(false)}
        />
      )}
    </div>
  );
}