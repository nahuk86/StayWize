import { useQuery } from '@tanstack/react-query';
import { dashboardService } from './dashboardService';

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <div className="bg-white rounded-lg shadow p-6 flex flex-col gap-2">
      <span className="text-sm text-gray-500 font-medium">{label}</span>
      <span className="text-3xl font-bold text-blue-700">{value}</span>
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
      <div className="flex items-center justify-center h-64">
        <span className="text-gray-500">Cargando...</span>
      </div>
    );
  }

  if (isError || !data) {
    return (
      <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md">
        Error al cargar el dashboard.
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800 mb-6">Dashboard</h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <StatCard label="Propiedades" value={data.totalProperties} />
        <StatCard label="Reservas totales" value={data.totalReservations} />
        <StatCard label="Reservas activas" value={data.activeReservations} />
        <StatCard label="Códigos generados" value={data.totalAccessCodes} />
        <StatCard label="Códigos activos" value={data.activeAccessCodes} />
      </div>
    </div>
  );
}