import { useForm, type SubmitHandler } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { reservationService, type CreateReservationDto } from './reservationService';
import { propertyService } from '../properties/propertyService';
import { clientService } from '../clients/clientService';
import { useState } from 'react';

const reservationSchema = z.object({
  propertyId: z.string().min(1, 'La propiedad es requerida'),
  clientId: z.string().min(1, 'El cliente es requerido'),
  checkIn: z.string().min(1, 'La fecha de check-in es requerida'),
  checkOut: z.string().min(1, 'La fecha de check-out es requerida'),
  guestCount: z.coerce.number().min(1, 'Debe haber al menos 1 huésped'),
  notes: z.string().optional(),
});

type ReservationForm = z.infer<typeof reservationSchema>;

interface Props {
  onClose: () => void;
}

export function ReservationFormModal({ onClose }: Props) {
  const queryClient = useQueryClient();

  const { data: properties } = useQuery({
    queryKey: ['properties'],
    queryFn: propertyService.getAll,
  });

  const { data: clients } = useQuery({
    queryKey: ['clients'],
    queryFn: clientService.getAll,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ReservationForm>({
    resolver: zodResolver(reservationSchema) as any,
  });

  const [serverError, setServerError] = useState<string | null>(null);

const createMutation = useMutation({
  mutationFn: (data: CreateReservationDto) => reservationService.create(data),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['reservations'] });
    onClose();
  },
  onError: (error: any) => {
    setServerError(error.response?.data?.message ?? 'Error al crear la reserva.');
  },
});
  

  const onSubmit: SubmitHandler<ReservationForm> = (data) => {
    createMutation.mutate({
      ...data,
      checkIn: new Date(data.checkIn).toISOString(),
      checkOut: new Date(data.checkOut).toISOString(),
    });
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-md p-6 max-h-[90vh] overflow-y-auto">
        <h2 className="text-lg font-bold text-slate-800 mb-4">Nueva reserva</h2>
        <form onSubmit={handleSubmit(onSubmit as any)} className="space-y-3">

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Propiedad</label>
            <select
              {...register('propertyId')}
              className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Seleccioná una propiedad</option>
              {properties?.map((p) => (
                <option key={p.id} value={p.id}>{p.name}</option>
              ))}
            </select>
            {errors.propertyId && <p className="text-red-500 text-xs mt-1">{errors.propertyId.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Cliente</label>
            <select
              {...register('clientId')}
              className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Seleccioná un cliente</option>
              {clients?.map((c) => (
                <option key={c.id} value={c.id}>{c.firstName} {c.lastName}</option>
              ))}
            </select>
            {errors.clientId && <p className="text-red-500 text-xs mt-1">{errors.clientId.message}</p>}
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Check-in</label>
              <input
                {...register('checkIn')}
                type="date"
                className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              {errors.checkIn && <p className="text-red-500 text-xs mt-1">{errors.checkIn.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Check-out</label>
              <input
                {...register('checkOut')}
                type="date"
                className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              {errors.checkOut && <p className="text-red-500 text-xs mt-1">{errors.checkOut.message}</p>}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Cantidad de huéspedes</label>
            <input
              {...register('guestCount')}
              type="number"
              min={1}
              className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            {errors.guestCount && <p className="text-red-500 text-xs mt-1">{errors.guestCount.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Notas (opcional)</label>
            <textarea
              {...register('notes')}
              rows={2}
              className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
            />
          </div>
            {serverError && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md text-sm">
                {serverError}
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
              disabled={createMutation.isPending}
              className="flex-1 bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 disabled:opacity-50 text-sm font-medium"
            >
              {createMutation.isPending ? 'Creando...' : 'Crear reserva'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}