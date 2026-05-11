import { useQuery } from '@tanstack/react-query';
import { dashboardService } from './dashboardService';

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <div className="bg-white rounded-lg border border-gray-200 p-5 flex flex-col gap-1 shadow-sm">
      <span className="text-xs font-semibold text-gray-500 uppercase tracking-wide">{label}</span>
      <span className="text-3xl font-bold text-slate-800">{value}</span>
    </div>
  );
}

export function DashboardPage() {
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
    </div>
  );
}