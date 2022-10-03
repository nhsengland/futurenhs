const { alpha, lighten, darken } = require('./helpers/util/tailwind/color')
const {
    generateBorderSides,
} = require('./helpers/util/tailwind/plugins/border-sides')

const themeColors = {
    ['theme-0']: 'rgba(0, 0, 0, 1)',
    ['theme-1']: '#fff',
    ['theme-2']: 'rgba(247, 249, 250, 1)',
    ['theme-3']: '#f0f4f5',
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
    ['theme-20']: '#006747',
}

//add darker and lighter tints to colors
Object.keys(themeColors).map((colorName) => {
    themeColors[colorName] = {
        DEFAULT: themeColors[colorName],
        lighter: lighten(themeColors[colorName], 0.1),
        darker: darken(themeColors[colorName], 0.25),
        75: alpha(themeColors[colorName], 0.75),
    }
})

module.exports = {
    mode: 'jit',
    prefix: 'u-',
    content: [
        './pages/**/*.js',
        './pages/**/*.ts',
        './pages/**/*.jsx',
        './pages/**/*.tsx',
        './components/**/*.js',
        './components/**/*.ts',
        './components/**/*.jsx',
        './components/**/*.tsx',
        './config/form-configs/**/*.ts',
    ],
    safelist: [
        {
            pattern: /bg-theme-\d{1,2}$/,
        },
        {
            pattern: /text-theme-\d{1,2}$/,
        },
        {
            pattern: /text-theme-\d{1,2}$/,
        },
        {
            pattern: /border-theme-\d{1,2}$/,
        },
        {
            pattern: /fill-theme-\d{1,2}$/,
        },
        {
            pattern: /border-[lrtb]-theme-\d{1,2}-?\w+$/g,
        },
    ],
    theme: {
        screens: {
            tablet: '768px',
            desktop: '960px',
            'desktop-large': '1446px',
        },
        colors: themeColors,
        fill: (theme) => theme('colors'),
        stroke: (theme) => theme('colors'),
    },
    variants: {
        extend: {
            border: ['hover', 'DEFAULT'],
            // IF you wish to extend variants for border-{side}-theme-n, then look at how the plugin adds the utilities
        },
    },
    plugins: [generateBorderSides],
}
