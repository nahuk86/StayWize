import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { dashboardService } from './dashboardService';
import { registrationRequestService } from '../registration-requests/registrationRequestService';
import type { RegistrationRequestDto } from '../registration-requests/registrationRequestService';
import { useAuth } from '../../hooks/useAuth';

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <div className="bg-white rounded-lg border border-gray-200 p-5 flex flex-col gap-1 shadow-sm">
      <span className="text-xs font-semibold text-gray-500 uppercase tracking-wide">{label}</span>
      <span className="text-3xl font-bold text-slate-800">{value}</span>
    </div>
  );
}

function PendingRequestsSection() {
  const queryClient = useQueryClient();
  const [approvingId, setApprovingId] = useState<string | null>(null);

  const { data: requests, isLoading, isError } = useQuery({
    queryKey: ['registration-requests-pending'],
    queryFn: registrationRequestService.getPending,
  });

  const approveMutation = useMutation({
    mutationFn: (id: string) => registrationRequestService.approve(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['registration-requests-pending'] });
      setApprovingId(null);
    },
    onError: () => setApprovingId(null),
  });

  const handleApprove = (id: string) => {
    if (!confirm('¿Confirmar el alta de este cliente? Se le enviará un email para completar su registro.')) return;
    setApprovingId(id);
    approveMutation.mutate(id);
  };

  if (isLoading) {
    return (
      <div className="text-gray-400 text-sm py-4">Cargando solicitudes...</div>
    );
  }

  if (isError) {
    return (
      <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md text-sm">
        Error al cargar solicitudes pendientes.
      </div>
    );
  }

  const pendingCount = requests?.length ?? 0;

  return (
    <div className="mt-8">
      <div className="flex items-center gap-3 mb-4">
        <h2 className="text-base font-bold text-slate-800">Solicitudes de alta pendientes</h2>
        {pendingCount > 0 && (
          <span className="bg-amber-100 text-amber-700 border border-amber-200 text-xs font-semibold px-2 py-0.5 rounded-full">
            {pendingCount}
          </span>
        )}
      </div>

      {pendingCount === 0 ? (
        <div className="bg-white border border-gray-200 rounded-lg p-6 text-center text-gray-400 text-sm shadow-sm">
          No hay solicitudes pendientes.
        </div>
      ) : (
        <>
          {/* Tabla — desktop */}
          <div className="hidden md:block bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 text-gray-500 uppercase text-xs border-b border-gray-200">
                <tr>
                  <th className="px-4 py-3 text-left">Nombre</th>
                  <th className="px-4 py-3 text-left">Email</th>
                  <th className="px-4 py-3 text-left">Documento</th>
                  <th className="px-4 py-3 text-left">Teléfono</th>
                  <th className="px-4 py-3 text-left">Fecha</th>
                  <th className="px-4 py-3 text-left">Acción</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {requests!.map((r: RegistrationRequestDto) => (
                  <tr key={r.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-3 font-medium text-slate-800">
                      {r.firstName} {r.lastName}
                    </td>
                    <td className="px-4 py-3 text-gray-600">{r.email}</td>
                    <td className="px-4 py-3 text-gray-600">{r.documentNumber}</td>
                    <td className="px-4 py-3 text-gray-600">{r.phone}</td>
                    <td className="px-4 py-3 text-gray-500">
                      {new Date(r.createdAt).toLocaleDateString('es-AR')}
                    </td>
                    <td className="px-4 py-3">
                      <button
                        onClick={() => handleApprove(r.id)}
                        disabled={approvingId === r.id}
                        className="bg-blue-600 text-white text-xs font-medium px-3 py-1.5 rounded-md hover:bg-blue-700 disabled:opacity-50 transition-colors"
                      >
                        {approvingId === r.id ? 'Procesando…' : 'Dar de alta'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Cards — mobile */}
          <div className="md:hidden space-y-3">
            {requests!.map((r: RegistrationRequestDto) => (
              <div key={r.id} className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm">
                <div className="flex items-start justify-between mb-2">
                  <p className="font-semibold text-slate-800 text-sm">
                    {r.firstName} {r.lastName}
                  </p>
                  <span className="text-xs text-gray-400">
                    {new Date(r.createdAt).toLocaleDateString('es-AR')}
                  </span>
                </div>
                <p className="text-xs text-gray-500 mb-0.5">{r.email}</p>
                <p className="text-xs text-gray-500 mb-0.5">DNI: {r.documentNumber}</p>
                <p className="text-xs text-gray-500 mb-3">Tel: {r.phone}</p>
                <button
                  onClick={() => handleApprove(r.id)}
                  disabled={approvingId === r.id}
                  className="w-full bg-blue-600 text-white text-xs font-medium px-3 py-2 rounded-md hover:bg-blue-700 disabled:opacity-50 transition-colors"
                >
                  {approvingId === r.id ? 'Procesando…' : 'Dar de alta'}
                </button>
              </div>
            ))}
          </div>
        </>
      )}
    </div>
  );
}

export function DashboardPage() {
  const { user } = useAuth();
  const isAdminOrOwner = user?.role === 'Admin' || user?.role === 'Owner';

  const { data, isLoading, isError } = useQuery({
    queryKey: ['dashboard-global'],
    queryFn: dashboardService.getGlobal,
  });

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-48">
        <span className="text-gray-400 text-sm">Cargando...</span>
      </div>
    );
  }

  if (isError || !data) {
    return (
      <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md text-sm">
        Error al cargar el dashboard.
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-xl font-bold text-slate-800 mb-1">Dashboard</h1>
      <p className="text-sm text-gray-500 mb-6">Resumen general del sistema</p>

      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4">
        <StatCard label="Propiedades" value={data.totalProperties} />
        <StatCard label="Reservas totales" value={data.totalReservations} />
        <StatCard label="Reservas activas" value={data.activeReservations} />
        <StatCard label="Códigos generados" value={data.totalAccessCodes} />
        <StatCard label="Códigos activos" value={data.activeAccessCodes} />
      </div>

      {isAdminOrOwner && <PendingRequestsSection />}
    </div>
  );
}