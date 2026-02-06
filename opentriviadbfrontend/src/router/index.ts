import { createRouter, createWebHistory } from 'vue-router';
import StartView from '@/views/StartView.vue';
import QuizView from '@/views/QuizView.vue';
import ResultView from '@/views/ResultView.vue';
import { useQuizStore } from '@/stores/quiz';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'start',
      component: StartView,
    },
    {
      path: '/quiz',
      name: 'quiz',
      component: QuizView,
      beforeEnter: () => {
        const quizStore = useQuizStore();
        if (!quizStore.hasQuiz) {
          return '/';
        }
      },
    },
    {
      path: '/result',
      name: 'result',
      component: ResultView,
      beforeEnter: () => {
        const quizStore = useQuizStore();
        if (!quizStore.hasQuiz || !quizStore.isQuizComplete) {
          return '/';
        }
      },
    },
  ],
});

export default router;
