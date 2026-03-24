import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
      proxy: {
          '/weatherforecast': {
              target: 'http://localhost:5094',
              changeOrigin: true,
              secure: false,
              rewrite: (path) => path.replace(/^\/weatherforecast/, '/weatherforecast')
          }
      }
  }
})
