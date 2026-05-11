import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { propertyService, type PropertyDto } from './propertyService';
import { useAuth } from '../../hooks/useAuth';

const propertySchema = z.object({
  name: z.string().min(1, 'El nombre es requerido'),
  address: z.string().min(1, 'La dirección es requerida'),
  city: z.string().min(1, 'La ciudad es requerida'),
  country: z.string().min(1, 'El país es requerido'),
  maxGuests: z.coerce.number().min(1, 'Debe tener al menos 1 huésped'),
});

type PropertyForm = z.infer<typeof propertySchema>;

interface Props {
  property: PropertyDto | null;
  onClose: () => void;
}

export function PropertyFormModal({ property, onClose }: Props) {
  const queryClient = useQueryClient();
  const { user } = useAuth();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<PropertyForm>({
    resolver: zodResolver(propertySchema),
    defaultValues: property ?? undefined,
  });

  const createMutation = useMutation({
    mutationFn: (data: PropertyForm) =>
        propertyService.create({ ...data, ownerId: user?.userId ?? '' }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['properties'] });
      onClose();
    },
  });

  const updateMutation = useMutation({
    mutationFn: (data: PropertyForm) =>
      propertyService.update(property!.id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['properties'] });
      onClose();
    },
  });

  const onSubmit = (data: PropertyForm) => {
    if (property) {
      updateMutation.mutate(data);
    } else {
      createMutation.mutate(data);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-md p-6">
        <h2 className="text-lg font-bold text-gray-800 mb-4">
          {property ? 'Editar propiedad' : 'Nueva propiedad'}
        </h2>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
          {(['name', 'address', 'city', 'country'] as const).map((field) => (
            <div key={field}>
              <label className="block text-sm font-medium text-gray-700 mb-1 capitalize">
                {field === 'name' ? 'Nombre' : field === 'address' ? 'Dirección' : field === 'city' ? 'Ciudad' : 'País'}
              </label>
              <input
                {...register(field)}
                className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              {errors[field] && (
                <p className="text-red-500 text-xs mt-1">{errors[field]?.message}</p>
              )}
            </div>
          ))}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Huéspedes máximos
            </label>
            <input
              {...register('maxGuests')}
              type="number"
              className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            {errors.maxGuests && (
              <p className="text-red-500 text-xs mt-1">{errors.maxGuests.message}</p>
            )}
          </div>

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
              disabled={isSubmitting}
              className="flex-1 bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 disabled:opacity-50 text-sm font-medium"
            >
              {property ? 'Guardar cambios' : 'Crear'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}