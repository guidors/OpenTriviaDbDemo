<script setup lang="ts">
</script>

<template>
  <div id="app">
    <header class="bg-blue-600 text-white p-4">
      
    </header>
    
    <main class="px-4 py-8">
      <div class="mx-auto max-w-4xl">
        <RouterView />
      </div>
    </main>
  </div>
</template>

<style>
/* Force vertical layout */
#app {
  display: block !important;
  width: 100% !important;
  min-height: 100vh;
  background-color: #fef7ed;
}

#app header {
  display: block !important;
  width: 100% !important;
}

#app main {
  display: block !important;
  width: 100% !important;
  max-width: 800px;
  margin: 0 auto;
}

/* Global styles */
body {
  margin: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.container {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

h1, h2, h3, h4, h5, h6 {
  color: #1f2937;
  font-weight: 600;
}

/* Basic button styles */
.btn {
  display: inline-block;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.375rem;
  font-weight: 500;
  text-decoration: none;
  cursor: pointer;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background-color: #2563eb;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #1d4ed8;
}

.btn-secondary {
  background-color: #6b7280;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #4b5563;
}

.btn-success {
  background-color: #059669;
  color: white;
}

.btn-success:hover:not(:disabled) {
  background-color: #047857;
}

.btn-danger {
  background-color: #dc2626;
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background-color: #b91c1c;
}

/* Form styles */
.form-group {
  margin-bottom: 1rem;
}

.form-label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
  color: #1f2937;
}

.form-input, .form-select {
  display: block;
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 1rem;
}

.form-input:focus, .form-select:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

/* Card styles */
.card {
  background: #fffef7;
  border-radius: 0.5rem;
  padding: 1.5rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  border: 1px solid #fbbf24;
}

/* Utility classes */
.text-center { text-align: center; }
.text-left { text-align: left; }
.text-right { text-align: right; }

.mb-1 { margin-bottom: 0.25rem; }
.mb-2 { margin-bottom: 0.5rem; }
.mb-3 { margin-bottom: 0.75rem; }
.mb-4 { margin-bottom: 1rem; }
.mb-6 { margin-bottom: 1.5rem; }
.mb-8 { margin-bottom: 2rem; }

.mt-1 { margin-top: 0.25rem; }
.mt-2 { margin-top: 0.5rem; }
.mt-3 { margin-top: 0.75rem; }
.mt-4 { margin-top: 1rem; }
.mt-6 { margin-top: 1.5rem; }
.mt-8 { margin-top: 2rem; }

.p-2 { padding: 0.5rem; }
.p-4 { padding: 1rem; }
.p-6 { padding: 1.5rem; }

.flex { display: flex; }
.flex-col { flex-direction: column; }
.items-center { align-items: center; }
.justify-center { justify-content: center; }
.justify-between { justify-content: space-between; }
.gap-2 { gap: 0.5rem; }
.gap-4 { gap: 1rem; }

.w-full { width: 100%; }
.max-w-md { max-width: 28rem; }
.max-w-2xl { max-width: 42rem; }

.bg-white { background-color: #fffef7; }
.bg-gray-50 { background-color: #fef7ed; }
.bg-gray-100 { background-color: #fed7aa; }
.bg-blue-50 { background-color: #eff6ff; }
.bg-green-100 { background-color: #dcfce7; }
.bg-red-100 { background-color: #fee2e2; }

.text-green-800 { color: #166534; }
.text-red-800 { color: #991b1b; }
.text-gray-600 { color: #1f2937; }
.text-gray-800 { color: #111827; }

/* Font sizes */
.text-sm { font-size: 0.875rem; }
.text-base { font-size: 1rem; }
.text-lg { font-size: 1.125rem; }
.text-xl { font-size: 1.25rem; }
.text-2xl { font-size: 1.5rem; }
.text-3xl { font-size: 1.875rem; }
.text-6xl { font-size: 3.75rem; }

/* Font weights */
.font-medium { font-weight: 500; }
.font-semibold { font-weight: 600; }
.font-bold { font-weight: 700; }

.border { border: 1px solid #e5e7eb; }
.border-green-300 { border-color: #86efac; }
.border-red-300 { border-color: #fca5a5; }

.rounded { border-radius: 0.25rem; }
.rounded-lg { border-radius: 0.5rem; }

.shadow-sm { box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05); }

.min-h-screen { min-height: 100vh; }
</style>
