import type { QuizRequest, QuizResponse, AnswerRequest, AnswerResponse, CategoryResponse } from '@/types';

const BASE_URL = 'http://localhost:5002'; // Backend URL

class ApiService {
  private async request<T>(url: string, options: RequestInit = {}): Promise<T> {
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
      ...options,
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`API Error: ${response.status} - ${errorText}`);
    }

    return response.json();
  }

  async getCategories(): Promise<CategoryResponse> {
    return this.request<CategoryResponse>(`${BASE_URL}/OpenTriviaDbWebService/get_categories`);
  }

  async getQuiz(config: QuizRequest): Promise<QuizResponse> {
    return this.request<QuizResponse>(`${BASE_URL}/OpenTriviaDbWebService/get_quiz`, {
      method: 'POST',
      body: JSON.stringify(config),
    });
  }

  async checkAnswer(request: AnswerRequest): Promise<AnswerResponse> {
    return this.request<AnswerResponse>(`${BASE_URL}/OpenTriviaDbWebService/check_answers`, {
      method: 'POST',
      body: JSON.stringify(request),
    });
  }
}

export const apiService = new ApiService();
