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
            "./components/**/*.tsx",
            "./helpers/renderers/**/*.tsx"
        ]
    },    
    darkMode: false,
    theme: {
        screens: {
            'tablet': '768px',      
            'desktop': '960px',
            'desktop-large': '1446px',
        }
    },
    variants: {
        extend: {},
    },
    plugins: [],
}