module.exports = {
    prefix: 'u-',
    purge: {
        enabled: true,
        layers: ["base", "components", "utilities"],
        content: [
            "./pages/**/*.js",
            "./pages/**/*.ts",
            "./pages/**/*.jsx",
            "./pages/**/*.tsx",
            "./components/**/*.js",
            "./components/**/*.ts",
            "./components/**/*.jsx",
            "./components/**/*.tsx"
        ],
        safelist: [/bg-theme-/, /text-theme-/]
    },
    darkMode: false,
    theme: {
        screens: {
            'tablet': '768px',
            'desktop': '960px',
            'desktop-large': '1446px',
        },
        /**
         * TODO - ideally import these directly from SASS (Next won't let us by default unless it's into /pages/_app)
         */
        colors: {
            ['theme-0']: 'rgba(0, 0, 0, 1)',
            ['theme-1']: '#fff',
            ['theme-2']: 'rgba(247, 249, 250, 1)',
            ['theme-3']: 'rgba(246, 246, 246, 1)',
            ['theme-4']: '#d8dde0',
            ['theme-5']: '#aeb7bd',
            ['theme-6']: '#768692',
            ['theme-7']: '#4c6272',
            ['theme-8']: '#005eb8',
            ['theme-9']: '#007f3b',
            ['theme-10']: '#330072',
            ['theme-11']: '#00A499',
            ['theme-12']: 'rgba(138, 21, 56, 1)',
            ['theme-13']: '#d5281b',
            ['theme-14']: 'rgba(0, 48, 135, 1)',
            ['theme-15']: '#ffeb3b',
            ['theme-16']: '#41b6e6',
            ['theme-17']: '#ed8b00',
            ['theme-18']: '#ffb81c',
            ['theme-19']: '#78be20',
            ['theme-20']: '#006747'
        }
    },
    variants: {
        extend: {},
    },
    plugins: [],
}