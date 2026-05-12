import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { accessCodeService } from './accessCodeService';

interface Props {
  reservationId: string;
  onClose: () => void;
}

export function GenerateAccessCodeModal({ reservationId, onClose }: Props) {
  const queryClient = useQueryClient();
  const [validFrom, setValidFrom] = useState('');
  const [validTo, setValidTo] = useState('');
  const [type, setType] = useState(1);
  const [error, setError] = useState<string | null>(null);

  const generateMutation = useMutation({
    mutationFn: () => accessCodeService.generate({
      reservationId,
      validFrom: new Date(validFrom).toISOString(),
      validTo: new Date(validTo).toISOString(),
      type,
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['access-codes', reservationId] });
      onClose();
    },
    onError: (err: any) => {
      setError(err.response?.data?.message ?? 'Error al generar el código.');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    generateMutation.mutate();
  };

return (
<div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4 overflow-y-auto">
  <div className="bg-white rounded-lg shadow-xl w-full sm:max-w-md p-6 my-auto">      <h2 className="text-lg font-bold text-slate-800 mb-4">Generar código de acceso</h2>
      <form onSubmit={handleSubmit} className="space-y-4">

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Tipo</label>
          <select
            value={type}
            onChange={(e) => setType(Number(e.target.value))}
            className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value={1}>Check-in</option>
            <option value={2}>Check-out</option>
            <option value={3}>General</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Válido desde</label>
          <input
            type="datetime-local"
            value={validFrom}
            onChange={(e) => setValidFrom(e.target.value)}
            required
            className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Válido hasta</label>
          <input
            type="datetime-local"
            value={validTo}
            onChange={(e) => setValidTo(e.target.value)}
            required
            className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
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
            className="flex-1 border border-gray-300 text-gray-700 py-2.5 rounded-md hover:bg-gray-50 text-sm font-medium"
          >
            Cancelar
          </button>
          <button
            type="submit"
            disabled={generateMutation.isPending}
            className="flex-1 bg-blue-600 text-white py-2.5 rounded-md hover:bg-blue-700 disabled:opacity-50 text-sm font-medium"
          >
            {generateMutation.isPending ? 'Generando...' : 'Generar'}
          </button>
        </div>
      </form>
    </div>
  </div>
);
}