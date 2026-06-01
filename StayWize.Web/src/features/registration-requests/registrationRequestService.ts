import apiClient from '../../api/client';

export interface CreateRegistrationRequestDto {
  firstName:      string;
  lastName:       string;
  email:          string;
  documentNumber: string;
  phone:          string;
}

export interface RegistrationRequestDto {
  id:             string;
  firstName:      string;
  lastName:       string;
  email:          string;
  documentNumber: string;
  phone:          string;
  status:         string;
  createdAt:      string;
}

export const registrationRequestService = {
  create: async (dto: CreateRegistrationRequestDto): Promise<RegistrationRequestDto> => {
    const response = await apiClient.post('/RegistrationRequests', dto);
    return response.data;
  },

  getPending: async (): Promise<RegistrationRequestDto[]> => {
    const response = await apiClient.get('/RegistrationRequests/pending');
    return response.data;
  },

  approve: async (id: string): Promise<void> => {
    await apiClient.post(`/RegistrationRequests/${id}/approve`);
  },
};