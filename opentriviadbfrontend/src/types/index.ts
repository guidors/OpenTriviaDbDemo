export interface QuizRequest {
  numberOfQuestions: number;
  category: number;
  quizType: 'any' | 'multiple' | 'boolean';
  difficulty: 'any' | 'easy' | 'medium' | 'hard';
}

export interface QuizItem {
  type: string;
  difficulty: string;
  category: string;
  question: string;
  answers: string[];
}

export interface QuizResponse {
  token: string;
  quiz_items: QuizItem[];
}

export interface AnswerRequest {
  token: string;
  question_id: number;
  question: string;
  answer: string;
}

export interface QuizScore {
  total: number;
  total_correct: number;
  total_incorrect: number;
}

export interface AnswerResponse {
  question_id: number;
  question: string;
  given_answer: string;
  correct_answer: string;
  is_correct: boolean;
  quiz_score: QuizScore;
}

export interface Category {
  id: number;
  name: string;
}

export interface CategoryResponse {
  trivia_categories: Category[];
}

// Helper enums for better developer experience
export enum QuizDifficulty {
  Any = 'any',
  Easy = 'easy',
  Medium = 'medium',
  Hard = 'hard'
}

export enum QuizType {
  Any = 'any',
  Multiple = 'multiple',
  Boolean = 'boolean'
}
