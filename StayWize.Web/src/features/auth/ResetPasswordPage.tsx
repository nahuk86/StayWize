import { useState } from 'react';
import { useSearchParams, useNavigate, Link } from 'react-router-dom';
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

export function ResetPasswordPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get('token');
  const email = searchParams.get('email');
  const [serverError, setServerError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  if (!token || !email) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="bg-white p-8 rounded-lg shadow text-center w-full max-w-md">
          <p className="text-red-600 font-medium mb-4">Link inválido — faltan parámetros.</p>
          <Link to="/forgot-password" className="text-blue-600 text-sm hover:underline">
            Solicitá un nuevo enlace
          </Link>
        </div>
      </div>
    );
  }

  const onSubmit = async (data: FormData) => {
    setServerError(null);
    try {
      await apiClient.post('/auth/reset-password', {
        email,
        token,
        password: data.password,
        confirmPassword: data.confirmPassword,
      });
      setSuccess(true);
      setTimeout(() => navigate('/login'), 2500);
    } catch (err: any) {
      setServerError(
        err.response?.data?.detail ?? 'El enlace es inválido o expiró. Solicitá uno nuevo.'
      );
    }
  };

  if (success) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="bg-white p-8 rounded-lg shadow w-full max-w-md text-center">
          <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">¡Contraseña actualizada!</h2>
          <p className="text-gray-500 text-sm">Redirigiendo al inicio de sesión…</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="bg-white p-8 rounded-lg shadow w-full max-w-md">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Nueva contraseña</h1>
          <p className="text-gray-500 text-sm mt-1">Elegí una contraseña segura para tu cuenta.</p>
        </div>

        {serverError && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">
            {serverError}
            <div className="mt-2">
              <Link to="/forgot-password" className="text-blue-600 font-medium hover:underline">
                Solicitá un nuevo enlace
              </Link>
            </div>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Nueva contraseña</label>
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
            <label className="block text-sm font-medium text-gray-700 mb-1">Confirmar contraseña</label>
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
            {isSubmitting ? 'Guardando...' : 'Guardar nueva contraseña'}
          </button>
        </form>
      </div>
    </div>
  );
}