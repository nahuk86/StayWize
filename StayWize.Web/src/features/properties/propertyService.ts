import apiClient from '../../api/client';

export interface PropertyDto {
  id: string;
  name: string;
  address: string;
  city: string;
  country: string;
  maxGuests: number;
  ownerId: string;
}

export interface CreatePropertyDto {
  name: string;
  address: string;
  city: string;
  country: string;
  maxGuests: number;
  ownerId: string;
}

export interface UpdatePropertyDto {
  name: string;
  address: string;
  city: string;
  country: string;
  maxGuests: number;
}

export const propertyService = {
  getAll: async (): Promise<PropertyDto[]> => {
    const response = await apiClient.get('/properties');
    return response.data;
  },
  getById: async (id: string): Promise<PropertyDto> => {
    const response = await apiClient.get(`/properties/${id}`);
    return response.data;
  },
  create: async (dto: CreatePropertyDto): Promise<PropertyDto> => {
    const response = await apiClient.post('/properties', dto);
    return response.data;
  },
  update: async (id: string, dto: UpdatePropertyDto): Promise<void> => {
    await apiClient.put(`/properties/${id}`, dto);
  },
  remove: async (id: string): Promise<void> => {
    await apiClient.delete(`/properties/${id}`);
  },
};