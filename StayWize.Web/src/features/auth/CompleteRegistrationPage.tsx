import { useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import apiClient from '../../api/client';

const schema = z.object({
  password: z.string().min(8, 'Mínimo 8 caracteres'),
  confirmPassword: z.string(),
}).refine(d => d.password === d.confirmPassword, {
  message: 'Las contraseñas no coinciden',
  path: ['confirmPassword'],
});

type FormData = z.infer<typeof schema>;

export function CompleteRegistrationPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get('token');
  const [serverError, setServerError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: FormData) => {
    if (!token) return;
    setServerError(null);
    try {
      await apiClient.post('/auth/complete-registration', {
        token,
        password: data.password,
        confirmPassword: data.confirmPassword,
      });
      setSuccess(true);
      setTimeout(() => navigate('/login'), 2000);
    } catch (err: any) {
      setServerError(
        err.response?.data?.detail ?? 'Token inválido o expirado.'
      );
    }
  };

  if (!token) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="bg-white p-8 rounded-lg shadow text-center">
          <p className="text-red-600 font-medium">Link inválido — falta el token.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="bg-white p-8 rounded-lg shadow w-full max-w-md">
        <h1 className="text-2xl font-bold text-gray-900 mb-2">Completar registro</h1>
        <p className="text-gray-500 text-sm mb-6">Elegí una contraseña para activar tu cuenta.</p>

        {success && (
          <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded text-green-700 text-sm">
            ¡Cuenta activada! Redirigiendo al login…
          </div>
        )}

        {serverError && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">
            {serverError}
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Contraseña
            </label>
            <input
              type="password"
              {...register('password')}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Mínimo 8 caracteres"
            />
            {errors.password && (
              <p className="text-red-500 text-xs mt-1">{errors.password.message}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Confirmar contraseña
            </label>
            <input
              type="password"
              {...register('confirmPassword')}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Repetí la contraseña"
            />
            {errors.confirmPassword && (
              <p className="text-red-500 text-xs mt-1">{errors.confirmPassword.message}</p>
            )}
          </div>

          <button
            type="submit"
            disabled={isSubmitting || success}
            className="w-full bg-blue-600 text-white py-2 px-4 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50 transition-colors"
          >
            {isSubmitting ? 'Activando cuenta…' : 'Activar cuenta'}
          </button>
        </form>
      </div>
    </div>
  );
}