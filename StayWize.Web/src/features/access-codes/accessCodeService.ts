import apiClient from '../../api/client';

export type AccessCodeStatus = 'Active' | 'Revoked' | 'Expired';
export type AccessCodeType = 'CheckIn' | 'CheckOut' | 'General';

const statusMap: Record<number, AccessCodeStatus> = {
  1: 'Active',
  2: 'Revoked',
  3: 'Expired',
};

const typeMap: Record<number, AccessCodeType> = {
  1: 'CheckIn',
  2: 'CheckOut',
  3: 'General',
};

export interface AccessCodeDto {
  id: string;
  reservationId: string;
  code: string;
  validFrom: string;
  validTo: string;
  status: AccessCodeStatus;
  type: AccessCodeType;
  createdAt: string;
}

export interface GenerateAccessCodeDto {
  reservationId: string;
  validFrom: string;
  validTo: string;
  type: number;
}

export const accessCodeService = {
    getByReservation: async (reservationId: string): Promise<AccessCodeDto[]> => {
    const response = await apiClient.get(`/access-codes/reservation/${reservationId}`);
    return response.data.map((c: any) => ({
        ...c,
        status: statusMap[c.status] ?? 'Active',
        type: typeMap[c.type] ?? 'CheckIn',
    }));
    },
  generate: async (dto: GenerateAccessCodeDto): Promise<AccessCodeDto> => {
    const response = await apiClient.post('/access-codes', dto);
    return {
      ...response.data,
      status: statusMap[response.data.status] ?? 'Active',
      type: typeMap[response.data.type] ?? 'CheckIn',
    };
  },
  revoke: async (id: string): Promise<void> => {
    await apiClient.patch(`/access-codes/${id}/revoke`);
  },
};