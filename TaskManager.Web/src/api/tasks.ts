import axios from 'axios'
import type { Task, CreateTaskRequest, UpdateStatusRequest, TaskStatus } from '@/types/task'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

export const tasksApi = {
  async getAll(): Promise<Task[]> {
    const response = await api.get<Task[]>('/tasks')
    return response.data
  },

  async create(request: CreateTaskRequest): Promise<Task> {
    const response = await api.post<Task>('/tasks', request)
    return response.data
  },

  async updateStatus(id: string, status: TaskStatus): Promise<void> {
    await api.patch(`/tasks/${id}/status`, { status })
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/tasks/${id}`)
  }
}