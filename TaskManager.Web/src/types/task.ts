export type TaskStatus = 'New' | 'InProgress' | 'Done'

export interface Task {
  id: string
  title: string
  status: TaskStatus
  createdAt: string
}

export interface CreateTaskRequest {
  title: string
}

export interface UpdateStatusRequest {
  status: TaskStatus
}