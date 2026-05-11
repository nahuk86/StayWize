import apiClient from '../../api/client';

export interface ClientDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  documentNumber: string;
}

export interface CreateClientDto {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  documentNumber: string;
}

export const clientService = {
  getAll: async (): Promise<ClientDto[]> => {
    const response = await apiClient.get('/clients');
    return response.data;
  },
  create: async (dto: CreateClientDto): Promise<ClientDto> => {
    const response = await apiClient.post('/clients', dto);
    return response.data;
  },
  update: async (id: string, dto: CreateClientDto): Promise<void> => {
    await apiClient.put(`/clients/${id}`, dto);
  },
  remove: async (id: string): Promise<void> => {
    await apiClient.delete(`/clients/${id}`);
  },
};