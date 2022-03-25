const generateBorderSides = ({ addUtilities, theme }) => {
    const themeColors = theme("colors");
    const colors = {};

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

    return Object.keys(colors);
};

module.exports = { generateBorderSides };