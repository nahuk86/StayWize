import apiClient from '../../api/client';

export interface GlobalDashboardDto {
  totalProperties: number;
  totalReservations: number;
  activeReservations: number;
  totalAccessCodes: number;
  activeAccessCodes: number;
}

export interface PropertyDashboardDto {
  propertyId: string;
  propertyName: string;
  totalReservations: number;
  activeReservations: number;
  totalAccessCodes: number;
  activeAccessCodes: number;
}

export const dashboardService = {
  getGlobal: async (): Promise<GlobalDashboardDto> => {
    const response = await apiClient.get('/dashboard/global');
    return response.data;
  },
  getByProperty: async (propertyId: string): Promise<PropertyDashboardDto> => {
    const response = await apiClient.get(`/dashboard/property/${propertyId}`);
    return response.data;
  },
};