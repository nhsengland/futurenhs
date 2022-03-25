module.exports = {
    plugins: {
        tailwindcss: {},
        autoprefixer: {},
        'postcss-pxtorem': {
            rootValue: 16,
            propList: ['*'],
            replace: true,
            mediaQuery: false
        }
    }
};