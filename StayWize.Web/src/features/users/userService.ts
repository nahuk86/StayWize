import apiClient from '../../api/client';

export type UserRole = 'Admin' | 'Owner' | 'HostLocal' | 'Guest';

export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
}

export interface CreateUserDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: UserRole;
}

export interface UpdateUserDto {
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
}

export const userService = {
  getAll: async (): Promise<UserDto[]> => {
    const response = await apiClient.get('/users');
    return response.data;
  },
  create: async (dto: CreateUserDto): Promise<void> => {
    await apiClient.post('/users', dto);
  },
  update: async (id: string, dto: UpdateUserDto): Promise<void> => {
    await apiClient.put(`/users/${id}`, dto);
  },
  remove: async (id: string): Promise<void> => {
    await apiClient.delete(`/users/${id}`);
  },
};