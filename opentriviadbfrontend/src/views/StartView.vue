<template>
  <div class="card max-w-md mx-auto">
    <h2 class="text-2xl font-bold mb-6 text-center">Start a New Quiz</h2>
    
    <div v-if="quizStore.error" class="mb-4 p-4 bg-red-100 border border-red-300 text-red-800 rounded">
      {{ quizStore.error }}
    </div>
    
    <form @submit.prevent="startQuiz" class="space-y-4">
      <div class="form-group">
        <label for="numberOfQuestions" class="form-label">Number of Questions</label>
        <input 
          id="numberOfQuestions"
          v-model.number="quizConfig.numberOfQuestions" 
          type="number" 
          min="1" 
          max="50" 
          class="form-input"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="category" class="form-label">Category</label>
        <select 
          id="category"
          v-model.number="quizConfig.category" 
          class="form-select"
          required
        >
          <option value="0">Any Category</option>
          <option v-for="category in quizStore.categories" :key="category.id" :value="category.id">
            {{ category.name }}
          </option>
        </select>
      </div>
      
      <div class="form-group">
        <label for="difficulty" class="form-label">Difficulty</label>
        <select 
          id="difficulty"
          v-model="quizConfig.difficulty" 
          class="form-select"
          required
        >
          <option value="any">Any Difficulty</option>
          <option value="easy">Easy</option>
          <option value="medium">Medium</option>
          <option value="hard">Hard</option>
        </select>
      </div>
      
      <div class="form-group">
        <label for="quizType" class="form-label">Question Type</label>
        <select 
          id="quizType"
          v-model="quizConfig.quizType" 
          class="form-select"
          required
        >
          <option value="any">Any Type</option>
          <option value="multiple">Multiple Choice</option>
          <option value="boolean">True/False</option>
        </select>
      </div>
      
      <button 
        type="submit" 
        :disabled="quizStore.isLoading" 
        class="btn btn-primary w-full"
      >
        {{ quizStore.isLoading ? 'Loading...' : 'Start Quiz' }}
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useQuizStore } from '@/stores/quiz';
import type { QuizRequest } from '@/types';

const router = useRouter();
const quizStore = useQuizStore();

const quizConfig = reactive<QuizRequest>({
  numberOfQuestions: 10,
  category: 0,
  difficulty: 'any',
  quizType: 'any',
});

onMounted(async () => {
  if (quizStore.categories.length === 0) {
    await quizStore.loadCategories();
  }
});

const startQuiz = async () => {
  const success = await quizStore.startQuiz(quizConfig);
  if (success) {
    router.push('/quiz');
  }
};
</script>
