import { defineStore } from 'pinia';
import { apiService } from '@/services/api';
import type { QuizRequest, QuizItem, AnswerResponse, Category, QuizScore } from '@/types';

export interface QuizAnswer {
  questionId: number;
  answer: string;
  isCorrect: boolean;
  correctAnswer: string;
}

export const useQuizStore = defineStore('quiz', {
  state: () => ({
    // Quiz configuration
    categories: [] as Category[],
    
    // Current quiz data
    sessionToken: null as string | null,
    questions: [] as QuizItem[],
    currentQuestionIndex: 0,
    answers: {} as Record<number, QuizAnswer>,
    
    // Quiz results
    score: null as QuizScore | null,
    
    // Loading states
    isLoading: false,
    error: null as string | null,
  }),

  getters: {
    currentQuestion: (state) => state.questions[state.currentQuestionIndex],
    
    isLastQuestion: (state) => state.currentQuestionIndex === state.questions.length - 1,
    
    isFirstQuestion: (state) => state.currentQuestionIndex === 0,
    
    isCurrentQuestionAnswered: (state) => state.answers[state.currentQuestionIndex] !== undefined,
    
    totalQuestions: (state) => state.questions.length,
    
    hasQuiz: (state) => state.sessionToken !== null && state.questions.length > 0,
    
    isQuizComplete: (state) => {
      const hasQuiz = state.sessionToken !== null && state.questions.length > 0;
      if (!hasQuiz) return false;
      return Object.keys(state.answers).length === state.questions.length;
    },
  },

  actions: {
    async loadCategories() {
      try {
        this.isLoading = true;
        this.error = null;
        const response = await apiService.getCategories();
        this.categories = response.trivia_categories;
      } catch (error) {
        this.error = error instanceof Error ? error.message : 'Failed to load categories';
        console.error('Error loading categories:', error);
      } finally {
        this.isLoading = false;
      }
    },

    async startQuiz(config: QuizRequest) {
      try {
        this.isLoading = true;
        this.error = null;
        
        const response = await apiService.getQuiz(config);
        
        this.sessionToken = response.token;
        this.questions = response.quiz_items;
        this.currentQuestionIndex = 0;
        this.answers = {};
        this.score = null;
        
        return true;
      } catch (error) {
        this.error = error instanceof Error ? error.message : 'Failed to start quiz';
        console.error('Error starting quiz:', error);
        return false;
      } finally {
        this.isLoading = false;
      }
    },

    async submitAnswer(answer: string) {
      if (!this.sessionToken || !this.currentQuestion) {
        throw new Error('No active quiz session');
      }

      try {
        this.isLoading = true;
        this.error = null;

        const request = {
          token: this.sessionToken,
          question_id: this.currentQuestionIndex,
          question: this.currentQuestion.question,
          answer: answer,
        };

        const response: AnswerResponse = await apiService.checkAnswer(request);
        
        // Store the answer result
        this.answers[this.currentQuestionIndex] = {
          questionId: response.question_id,
          answer: response.given_answer,
          isCorrect: response.is_correct,
          correctAnswer: response.correct_answer,
        };
        
        // Update score
        this.score = response.quiz_score;
        
        return response;
      } catch (error) {
        this.error = error instanceof Error ? error.message : 'Failed to submit answer';
        console.error('Error submitting answer:', error);
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    nextQuestion() {
      if (this.currentQuestionIndex < this.questions.length - 1) {
        this.currentQuestionIndex++;
      }
    },

    previousQuestion() {
      if (this.currentQuestionIndex > 0) {
        this.currentQuestionIndex--;
      }
    },

    goToQuestion(index: number) {
      if (index >= 0 && index < this.questions.length) {
        this.currentQuestionIndex = index;
      }
    },

    resetQuiz() {
      this.sessionToken = null;
      this.questions = [];
      this.currentQuestionIndex = 0;
      this.answers = {};
      this.score = null;
      this.error = null;
    },

    clearError() {
      this.error = null;
    },
  },
});
