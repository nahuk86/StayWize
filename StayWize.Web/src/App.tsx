import { Routes, Route, Navigate } from 'react-router-dom';
import { LoginPage } from './features/auth/LoginPage';
import { ProtectedRoute } from './components/ProtectedRoute';
import { MainLayout } from './components/layout/MainLayout';
import { DashboardPage } from './features/dashboard/DashboardPage';
import { PropertiesPage } from './features/properties/PropertiesPage';
import { ClientsPage } from './features/clients/ClientsPage';
import { ReservationsPage } from './features/reservations/ReservationsPage';
import { AccessCodesPage } from './features/access-codes/AccessCodesPage';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
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
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

export default App;