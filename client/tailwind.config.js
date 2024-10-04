/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,js,jsx,ts,tsx}"],  // Scan all relevant files
  theme: {
    extend: {},
  },
  plugins: [require('daisyui')],
}