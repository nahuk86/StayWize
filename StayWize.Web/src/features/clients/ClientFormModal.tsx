import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { clientService, type ClientDto } from './clientService';

const clientSchema = z.object({
  firstName:      z.string().min(1, 'Requerido'),
  lastName:       z.string().min(1, 'Requerido'),
  documentNumber: z.string().min(6, 'Documento inválido'),
  email:          z.string().email('Email inválido'),
  phone:          z.string().optional(),
});

type ClientForm = z.infer<typeof clientSchema>;

interface Props {
  client: ClientDto | null;
  onClose: () => void;
}

const fields: { key: keyof ClientForm; label: string; type?: string }[] = [
  { key: 'firstName', label: 'Nombre' },
  { key: 'lastName', label: 'Apellido' },
  { key: 'email', label: 'Email', type: 'email' },
  { key: 'phone', label: 'Teléfono' },
  { key: 'documentNumber', label: 'Número de documento' },
];

export function ClientFormModal({ client, onClose }: Props) {
  const queryClient = useQueryClient();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<ClientForm>({
    resolver: zodResolver(clientSchema),
    defaultValues: client ?? undefined,
  });

  const createMutation = useMutation({
    mutationFn: clientService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clients'] });
      onClose();
    },
  });

  const updateMutation = useMutation({
    mutationFn: (data: ClientForm) => clientService.update(client!.id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clients'] });
      onClose();
    },
  });

  const onSubmit = (data: ClientForm) => {
    if (client) {
      updateMutation.mutate(data);
    } else {
      createMutation.mutate(data);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-md p-6">
        <h2 className="text-lg font-bold text-gray-800 mb-4">
          {client ? 'Editar cliente' : 'Nuevo cliente'}
        </h2>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
          {fields.map(({ key, label, type }) => (
            <div key={key}>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                {label}
              </label>
              <input
                {...register(key)}
                type={type ?? 'text'}
                className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              {errors[key] && (
                <p className="text-red-500 text-xs mt-1">{errors[key]?.message}</p>
              )}
            </div>
          ))}

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
              {client ? 'Guardar cambios' : 'Crear'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}