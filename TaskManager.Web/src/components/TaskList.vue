<script setup lang="ts">
import { ref } from 'vue'
import { useTasks } from '@/composables/useTasks'
import type { TaskStatus } from '@/types/task'

const { 
  tasks, 
  loading, 
  error, 
  successMessage,
  addTask, 
  changeStatus, 
  removeTask,
  getStatusClass,
  getStatusLabel
} = useTasks()

const newTitle = ref('')

const handleSubmit = () => {
  addTask(newTitle.value)
  newTitle.value = ''
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<template>
  <div class="task-manager">
    <h1>📋 Менеджер задач</h1>

    <!-- Форма создания -->
    <form @submit.prevent="handleSubmit" class="create-form">
      <input 
        v-model="newTitle" 
        type="text" 
        placeholder="Название новой задачи..." 
        required
        :disabled="loading"
      />
      <button type="submit" :disabled="loading">
        {{ loading ? '...' : 'Добавить' }}
      </button>
    </form>

    <!-- Сообщения -->
    <div v-if="loading" class="message loading">⏳ Загрузка...</div>
    <div v-if="error" class="message error">❌ {{ error }}</div>
    <div v-if="successMessage" class="message success">✅ {{ successMessage }}</div>

    <!-- Список задач -->
    <div v-if="!loading && tasks.length === 0" class="empty">
      📭 Нет задач. Создайте первую!
    </div>

    <ul v-else class="task-list">
      <li v-for="task in tasks" :key="task.id" class="task-item">
        <div class="task-info">
          <span class="task-title">{{ task.title }}</span>
          <span class="task-date">{{ formatDate(task.createdAt) }}</span>
        </div>
        
        <div class="task-actions">
          <select 
            :value="task.status" 
            @change="e => changeStatus(task.id, (e.target as HTMLSelectElement).value as TaskStatus)"
            :class="['status-select', getStatusClass(task.status)]"
            :disabled="loading"
          >
            <option value="New">Новая</option>
            <option value="InProgress">В работе</option>
            <option value="Done">Завершена</option>
          </select>
          
          <button 
            @click="removeTask(task.id)" 
            class="btn-delete"
            :disabled="loading"
            title="Удалить задачу"
          >
            🗑️
          </button>
        </div>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.task-manager {
  max-width: 800px;
  margin: 2rem auto;
  padding: 0 1rem;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

h1 {
  text-align: center;
  color: #2c3e50;
  margin-bottom: 2rem;
}

.create-form {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.create-form input {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 2px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.create-form input:focus {
  outline: none;
  border-color: #42b983;
}

.create-form button {
  padding: 0.75rem 1.5rem;
  background: #42b983;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
  transition: background 0.2s;
}

.create-form button:hover:not(:disabled) {
  background: #369970;
}

.create-form button:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.message {
  padding: 0.75rem 1rem;
  border-radius: 6px;
  margin-bottom: 1rem;
  text-align: center;
}

.message.loading {
  background: #fff3cd;
  color: #856404;
}

.message.error {
  background: #f8d7da;
  color: #721c24;
}

.message.success {
  background: #d4edda;
  color: #155724;
}

.empty {
  text-align: center;
  padding: 3rem;
  color: #888;
  font-size: 1.2rem;
}

.task-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.task-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  margin-bottom: 0.5rem;
  background: #f8f9fa;
  border-radius: 6px;
  border-left: 4px solid #ddd;
  transition: transform 0.2s, box-shadow 0.2s;
}

.task-item:hover {
  transform: translateX(4px);
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.task-item:has(.status-new) { border-left-color: #17a2b8; }
.task-item:has(.status-progress) { border-left-color: #ffc107; }
.task-item:has(.status-done) { border-left-color: #28a745; }

.task-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.task-title {
  font-weight: 600;
  color: #2c3e50;
}

.task-date {
  font-size: 0.85rem;
  color: #888;
}

.task-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.status-select {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 0.9rem;
  cursor: pointer;
}

.status-select.status-new { background: #e0f7fa; }
.status-select.status-progress { background: #fff8e1; }
.status-select.status-done { background: #e8f5e9; }

.btn-delete {
  padding: 0.5rem 0.75rem;
  background: transparent;
  border: 1px solid #dc3545;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-delete:hover:not(:disabled) {
  background: #dc3545;
  color: white;
}

.btn-delete:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>