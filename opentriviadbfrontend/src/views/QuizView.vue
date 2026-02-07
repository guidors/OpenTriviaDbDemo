<template>
  <div class="max-w-2xl mx-auto">
    <!-- Progress indicator -->
    <div class="mb-6">
      <div class="flex justify-between items-center mb-2">
        <span class="text-sm text-gray-600">Question {{ quizStore.currentQuestionIndex + 1 }} of {{ quizStore.totalQuestions }}</span>
        <span v-if="quizStore.score" class="text-sm text-gray-600">Score: {{ quizStore.score.total_correct }}/{{ quizStore.score.total }}</span>
      </div>
      <div class="w-full bg-gray-200 rounded-full h-2">
        <div 
          class="bg-blue-600 h-2 rounded-full transition-all duration-300" 
          :style="{ width: `${((quizStore.currentQuestionIndex + 1) / quizStore.totalQuestions) * 100}%` }"
        ></div>
      </div>
    </div>

    <!-- Question card -->
    <div class="card mb-6">
      <div v-if="quizStore.error" class="mb-4 p-4 bg-red-100 border border-red-300 text-red-800 rounded">
        {{ quizStore.error }}
      </div>

      <div v-if="currentQuestion" class="space-y-4">
        <!-- Question info -->
        <div class="text-sm text-gray-600 mb-2">
          <span class="font-medium">{{ currentQuestion.category }}</span> | 
          <span class="capitalize">{{ currentQuestion.difficulty }}</span> | 
          <span class="capitalize">{{ currentQuestion.type }}</span>
        </div>

        <!-- Question -->
        <h3 class="text-xl font-semibold mb-4" v-html="currentQuestion.question"></h3>

        <!-- Answer feedback (if answered) -->
        <div v-if="currentAnswer" class="mb-4 p-4 rounded" 
             :class="currentAnswer.isCorrect ? 'bg-green-100 border border-green-300' : 'bg-red-100 border border-red-300'">
          <p class="font-semibold" :class="currentAnswer.isCorrect ? 'text-green-800' : 'text-red-800'">
            {{ currentAnswer.isCorrect ? '✓ Correct!' : '✗ Incorrect' }}
          </p>
          <p v-if="!currentAnswer.isCorrect" class="text-sm mt-1 text-gray-700">
            Correct answer: <span class="font-medium" v-html="currentAnswer.correctAnswer"></span>
          </p>
        </div>

        <!-- Answer options -->
        <div class="space-y-2">
          <button
            v-for="(answer, index) in currentQuestion.answers"
            :key="index"
            @click="selectAnswer(answer)"
            :disabled="isAnswered || quizStore.isLoading"
            class="w-full p-4 text-left border rounded-lg transition-all"
            :class="getAnswerButtonClass(answer)"
            v-html="answer"
          />
        </div>
      </div>

      <div v-if="quizStore.isLoading" class="text-center py-8">
        <p>Loading...</p>
      </div>
    </div>

    <!-- Navigation buttons -->
    <div class="flex justify-between items-center">
      <button
        @click="previousQuestion"
        :disabled="quizStore.isFirstQuestion"
        class="btn btn-secondary"
      >
        Previous
      </button>

      <button
        v-if="!quizStore.isLastQuestion"
        @click="nextQuestion"
        :disabled="!isAnswered"
        class="btn btn-primary"
      >
        Next
      </button>

      <button
        v-else-if="isAnswered"
        @click="finishQuiz"
        class="btn btn-success"
      >
        {{ isAnswered ? 'Finish Quiz' : 'Answer' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { useQuizStore } from '@/stores/quiz';

const router = useRouter();
const quizStore = useQuizStore();

const currentQuestion = computed(() => quizStore.currentQuestion);
const currentAnswer = computed(() => quizStore.answers[quizStore.currentQuestionIndex]);
const isAnswered = computed(() => quizStore.isCurrentQuestionAnswered);

const selectAnswer = async (answer: string) => {
  if (!isAnswered.value && !quizStore.isLoading) {
    try {
      await quizStore.submitAnswer(answer);
    } catch (error) {
      console.error('Error submitting answer:', error);
    }
  }
};

const previousQuestion = () => {
  quizStore.previousQuestion();
};

const nextQuestion = () => {
  quizStore.nextQuestion();
};

const finishQuiz = () => {
  router.push('/result');
};

const getAnswerButtonClass = (answer: string) => {
  if (!currentAnswer.value) {
    return 'border-gray-300 hover:border-blue-300 hover:bg-blue-50 cursor-pointer';
  }
  
  if (answer === currentAnswer.value.correctAnswer) {
    return 'border-green-300 bg-green-100 text-green-800';
  }
  
  if (answer === currentAnswer.value.answer && !currentAnswer.value.isCorrect) {
    return 'border-red-300 bg-red-100 text-red-800';
  }
  
  return 'border-gray-300 bg-gray-50 text-gray-500 cursor-not-allowed';
};
</script>
