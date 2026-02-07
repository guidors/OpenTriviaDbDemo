<template>
  <div class="max-w-2xl mx-auto">
    <div class="card text-center">
      <h2 class="text-3xl font-bold mb-6">Quiz Complete!</h2>
      
      <div v-if="quizStore.score" class="mb-8">
        <!-- Score display -->
        <div class="text-6xl font-bold text-blue-600 mb-4">
          {{ Math.round((quizStore.score.total_correct / quizStore.score.total) * 100) }}%
        </div>
        
        <div class="text-xl text-gray-700 mb-6">
          You got {{ quizStore.score.total_correct }} out of {{ quizStore.score.total }} questions correct
        </div>
        
        <!-- Score bar -->
        <div class="w-full bg-gray-200 rounded-full h-4 mb-6">
          <div 
            class="h-4 rounded-full transition-all duration-500"
            :class="getScoreBarColor()"
            :style="{ width: `${(quizStore.score.total_correct / quizStore.score.total) * 100}%` }"
          ></div>
        </div>
        
        <!-- Score message -->
        <div class="text-lg font-medium mb-6" :class="getScoreMessageColor()">
          {{ getScoreMessage() }}
        </div>
      </div>

      <!-- Question review -->
      <div class="mb-8">
        <h3 class="text-xl font-semibold mb-4">Review Your Answers</h3>
        <div class="space-y-3">
          <div 
            v-for="(question, index) in quizStore.questions" 
            :key="index"
            class="flex items-center justify-between p-3 border rounded-lg cursor-pointer hover:bg-gray-50"
            @click="reviewQuestion(index)"
          >
            <span class="text-left flex-1" v-html="question.question"></span>
            <div class="flex items-center gap-2">
              <span 
                class="w-6 h-6 rounded-full flex items-center justify-center text-white text-sm font-bold"
                :class="quizStore.answers[index]?.isCorrect ? 'bg-green-500' : 'bg-red-500'"
              >
                {{ quizStore.answers[index]?.isCorrect ? '✓' : '✗' }}
              </span>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Action buttons -->
      <div class="flex gap-4 justify-center">
        <button
          @click="reviewAnswers"
          class="btn btn-secondary"
        >
          Review Answers
        </button>
        
        <button
          @click="startNewQuiz"
          class="btn btn-primary"
        >
          Start New Quiz
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useQuizStore } from '@/stores/quiz';

const router = useRouter();
const quizStore = useQuizStore();

const getScoreBarColor = () => {
  if (!quizStore.score) return 'bg-gray-400';
  
  const score = (quizStore.score.total_correct / quizStore.score.total) * 100;
  if (score >= 80) return 'bg-green-500';
  if (score >= 60) return 'bg-yellow-500';
  return 'bg-red-500';
};

const getScoreMessageColor = () => {
  if (!quizStore.score) return 'text-gray-600';
  
  const score = (quizStore.score.total_correct / quizStore.score.total) * 100;
  if (score >= 80) return 'text-green-600';
  if (score >= 60) return 'text-yellow-600';
  return 'text-red-600';
};

const getScoreMessage = () => {
  if (!quizStore.score) return '';
  
  const score = (quizStore.score.total_correct / quizStore.score.total) * 100;
  if (score === 100) return 'Perfect! Outstanding performance! 🎉';
  if (score >= 90) return 'Excellent work! 🌟';
  if (score >= 80) return 'Great job! 👏';
  if (score >= 70) return 'Good work! 👍';
  if (score >= 60) return 'Not bad! Keep practicing! 📚';
  if (score >= 50) return 'You can do better! Try again! 💪';
  return 'Keep studying and try again! 📖';
};

const reviewQuestion = (index: number) => {
  quizStore.goToQuestion(index);
  router.push('/quiz');
};

const reviewAnswers = () => {
  quizStore.goToQuestion(0);
  router.push('/quiz');
};

const startNewQuiz = () => {
  quizStore.resetQuiz();
  router.push('/');
};
</script>
