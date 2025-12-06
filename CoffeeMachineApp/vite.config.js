import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

export default defineConfig({
    plugins: [
        vue(),
        vueDevTools(),
    ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '/getCoffees': {
                target: 'https://localhost:7183',
                changeOrigin: true,
                secure: false 
            },
            '/getCoffeePricesInCents': {
                target: 'https://localhost:7183',
                changeOrigin: true,
                secure: false
            },
            '/getQuantity': {
                target: 'https://localhost:7183',
                changeOrigin: true,
                secure: false
            },
            '/buyCoffee': {
                target: 'https://localhost:7183',
                changeOrigin: true,
                secure: false
            }
        }
    }
})
