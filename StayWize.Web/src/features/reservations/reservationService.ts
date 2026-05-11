import apiClient from '../../api/client';

export type ReservationStatus = 'Pending' | 'Confirmed' | 'Cancelled';

const statusMap: Record<number, ReservationStatus> = {
  1: 'Pending',
  2: 'Confirmed',
  3: 'Cancelled',
};

export interface ReservationDto {
  id: string;
  propertyId: string;
  propertyName: string;
  clientId: string;
  clientName: string;
  checkIn: string;
  checkOut: string;
  guestCount: number;
  status: ReservationStatus;
  notes?: string;
}

export interface CreateReservationDto {
  propertyId: string;
  clientId: string;
  checkIn: string;
  checkOut: string;
  guestCount: number;
  notes?: string;
}

export const reservationService = {
  getAll: async (): Promise<ReservationDto[]> => {
    const response = await apiClient.get('/reservations');
    return response.data.map((r: any) => ({
      ...r,
      status: statusMap[r.status] ?? 'Pending',
    }));
  },
  create: async (dto: CreateReservationDto): Promise<ReservationDto> => {
    const response = await apiClient.post('/reservations', dto);
    return { ...response.data, status: statusMap[response.data.status] ?? 'Pending' };
  },
  confirm: async (id: string): Promise<void> => {
    await apiClient.patch(`/reservations/${id}/confirm`);
  },
  cancel: async (id: string): Promise<void> => {
    await apiClient.patch(`/reservations/${id}/cancel`);
  },
};