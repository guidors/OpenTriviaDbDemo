# OpenTrivia Quiz Application

## Development Setup

### Backend (.NET)
```bash
# Run the backend API
cd OpenTriviaDbWebService
dotnet run
```
Backend runs on: `https://localhost:7071`

### Frontend (Vue.js)
```bash
# Install dependencies (only needed once)
cd opentriviadbfrontend
npm install

# Run development server
npm run dev
```
Frontend runs on: `http://localhost:5173`

## Features

### ? Implemented Features
- **Quiz Configuration**: Select number of questions, category, difficulty, and type
- **Interactive Questions**: Multiple choice and true/false questions with visual feedback
- **Progress Tracking**: Visual progress bar and question counter
- **Answer Review**: See correct/incorrect answers immediately after answering
- **Navigation**: Go back to previous questions (answers are disabled when reviewing)
- **Final Score**: Percentage score with detailed breakdown
- **Answer Review**: Review all questions and answers at the end

### ?? Key Components
- **StartView**: Quiz configuration form with category dropdown
- **QuizView**: Question display with answer buttons and navigation
- **ResultView**: Final score display with answer review

### ?? Technical Features
- **TypeScript**: Fully typed Vue 3 application
- **Pinia**: State management for quiz data
- **Vue Router**: Client-side routing with route guards
- **Responsive Design**: Works on desktop and mobile
- **API Integration**: Seamless communication with .NET backend

## Usage

1. **Start Quiz**: Configure your quiz parameters on the home page
2. **Answer Questions**: Click answers and navigate through questions
3. **Review Results**: See your final score and review all answers

## API Endpoints

- `POST /OpenTriviaDbWebService/get_quiz`: Start a new quiz
- `POST /OpenTriviaDbWebService/check_answers`: Submit an answer

## Development Notes

- Backend uses CORS policy to allow frontend connections
- Frontend automatically decodes HTML entities from quiz questions
- Answer order is randomized by the backend
- Session tokens manage quiz state between frontend and backend