import { Routes, Route, Navigate } from 'react-router-dom';
import { LoginPage } from './features/auth/LoginPage';
import { CompleteRegistrationPage } from './features/auth/CompleteRegistrationPage';
import { ForgotPasswordPage } from './features/auth/ForgotPasswordPage';
import { ResetPasswordPage } from './features/auth/ResetPasswordPage';
import { RegisterRequestPage } from './features/registration-requests/RegisterRequestPage';
import { ProtectedRoute } from './components/ProtectedRoute';
import { MainLayout } from './components/layout/MainLayout';
import { DashboardPage } from './features/dashboard/DashboardPage';
import { PropertiesPage } from './features/properties/PropertiesPage';
import { ClientsPage } from './features/clients/ClientsPage';
import { ReservationsPage } from './features/reservations/ReservationsPage';
import { AccessCodesPage } from './features/access-codes/AccessCodesPage';
import { UsersPage } from './features/users/UsersPage';

function App() {
  return (
    <Routes>
      {/* Rutas públicas */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/complete-registration" element={<CompleteRegistrationPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPasswordPage />} />
      <Route path="/register-request" element={<RegisterRequestPage />} />

      {/* Rutas protegidas */}
      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <MainLayout>
              <DashboardPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/properties"
        element={
          <ProtectedRoute>
            <MainLayout>
              <PropertiesPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/clients"
        element={
          <ProtectedRoute>
            <MainLayout>
              <ClientsPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/reservations"
        element={
          <ProtectedRoute>
            <MainLayout>
              <ReservationsPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/access-codes"
        element={
          <ProtectedRoute>
            <MainLayout>
              <AccessCodesPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/users"
        element={
          <ProtectedRoute allowedRoles={['Admin', 'Owner']}>
            <MainLayout>
              <UsersPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

export default App;