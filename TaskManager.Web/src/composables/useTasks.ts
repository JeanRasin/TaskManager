import { ref, onMounted } from 'vue'
import type { Task, TaskStatus } from '@/types/task'
import { tasksApi } from '@/api/tasks'

export function useTasks() {
  const tasks = ref<Task[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const successMessage = ref<string | null>(null)

  const fetchTasks = async () => {
    loading.value = true
    error.value = null
    try {
      tasks.value = await tasksApi.getAll()
    } catch (e) {
      error.value = 'Не удалось загрузить задачи. Проверьте подключение к серверу.'
      console.error(e)
    } finally {
      loading.value = false
    }
  }

  const addTask = async (title: string) => {
    if (!title.trim()) {
      error.value = 'Название задачи не может быть пустым'
      return
    }
    try {
      const newTask = await tasksApi.create({ title: title.trim() })
      tasks.value.unshift(newTask)
      successMessage.value = 'Задача успешно создана'
      error.value = null
      setTimeout(() => (successMessage.value = null), 3000)
    } catch (e) {
      error.value = 'Не удалось создать задачу'
      console.error(e)
    }
  }

  const changeStatus = async (id: string, status: TaskStatus) => {
    try {
      await tasksApi.updateStatus(id, status)
      const task = tasks.value.find(t => t.id === id)
      if (task) {
        task.status = status
      }
      successMessage.value = 'Статус обновлён'
      setTimeout(() => (successMessage.value = null), 3000)
    } catch (e) {
      error.value = 'Не удалось обновить статус'
      console.error(e)
    }
  }

  const removeTask = async (id: string) => {
    try {
      await tasksApi.delete(id)
      tasks.value = tasks.value.filter(t => t.id !== id)
      successMessage.value = 'Задача удалена'
      setTimeout(() => (successMessage.value = null), 3000)
    } catch (e) {
      error.value = 'Не удалось удалить задачу'
      console.error(e)
    }
  }

  const getStatusClass = (status: TaskStatus): string => {
    switch (status) {
      case 'New': return 'status-new'
      case 'InProgress': return 'status-progress'
      case 'Done': return 'status-done'
      default: return ''
    }
  }

  const getStatusLabel = (status: TaskStatus): string => {
    switch (status) {
      case 'New': return 'Новая'
      case 'InProgress': return 'В работе'
      case 'Done': return 'Завершена'
      default: return status
    }
  }

  onMounted(fetchTasks)

  return {
    tasks,
    loading,
    error,
    successMessage,
    addTask,
    changeStatus,
    removeTask,
    refresh: fetchTasks,
    getStatusClass,
    getStatusLabel
  }
}