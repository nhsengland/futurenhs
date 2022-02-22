const Color = require('color')
const alpha = (clr, val) => Color(clr).alpha(val).rgb().string()
const lighten = (clr, val) => Color(clr).lighten(val).rgb().string()
const darken = (clr, val) => Color(clr).darken(val).rgb().string()

const _colors = {
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
};

const colorsWithTint = Object.keys(_colors).map(
    (colorName) => ({
        [colorName]: {
            DEFAULT: _colors[colorName], // => .bg-brand
            lighter: lighten(_colors[colorName], 0.1), // => .bg-brand-lighter
            darker: darken(_colors[colorName], 0.1), // => .bg-brand-darker
            '75': alpha(_colors[colorName], 0.75), // => .bg-brand-75
        }
    })
);

const generateBorderSidesPlugin = ({ e, addUtilities, theme }) => {
    const themeColors = theme("colors");
    const individualBorderColors = Object.keys(themeColors).map(
        (colorName) => {

            if (typeof (themeColors[colorName]) == 'string') {
                return ({
                    [`.border-b-${colorName}`]: {
                        borderBottomColor: themeColors[colorName],
                    },
                    [`.border-t-${colorName}`]: {
                        borderTopColor: themeColors[colorName],
                    },
                    [`.border-l-${colorName}`]: {
                        borderLeftColor: themeColors[colorName],
                    },
                    [`.border-r-${colorName}`]: {
                        borderRightColor: themeColors[colorName],
                    },
                })
            }

            const colors = {};

            Object.keys(themeColors[colorName]).forEach(level => {
                const variant = level.toLocaleLowerCase() === "default" ? '' : `-${level}`;

                colors[`.border-b-${colorName}${variant}`] = {
                    borderBottomColor: themeColors[colorName][level]
                }
                colors[`.border-t-${colorName}${variant}`] = {
                    borderTopColor: themeColors[colorName][level]
                }
                colors[`.border-l-${colorName}${variant}`] = {
                    borderLeftColor: themeColors[colorName][level]
                }
                colors[`.border-r-${colorName}${variant}`] = {
                    borderRightColor: themeColors[colorName][level]
                }

            });

            return colors;
        }
    );

    addUtilities(individualBorderColors, ['hover', 'DEFAULT']);
};

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
            "./form-configs/**/*.ts"
        ],
        safelist: [/bg-theme-/, /text-theme-/, /border-theme-/]
    },
    darkMode: false,
    theme: {
        screens: {
            'tablet': '768px',
            'desktop': '960px',
            'desktop-large': '1446px',
        },
        colors: Object.assign({}, ...colorsWithTint),
        fill: theme => theme('colors'),
        stroke: theme => theme('colors'),
    },
    variants: {
        extend: {
            border: ['hover', 'DEFAULT'],
            // IF you wish to extend variants for border-{side}-theme-n, then look at how the plugin adds the utilities
        }
    },
    plugins: [generateBorderSidesPlugin]
}