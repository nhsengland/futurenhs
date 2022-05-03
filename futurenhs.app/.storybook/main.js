const path = require('path');
const TsconfigPathsPlugin = require('tsconfig-paths-webpack-plugin');

module.exports = {
    stories: ['../components/**/*.stories.mdx', '../components/**/*.stories.@(js|jsx|ts|tsx)'],
    staticDirs: ['../public'],
    addons: [
        '@storybook/addon-links',
        '@storybook/addon-essentials',
        '@storybook/addon-a11y',
        '@storybook/addon-viewport',
        '@storybook/addon-actions',
        '@whitespace/storybook-addon-html',
        require.resolve('storybook-addon-grid/preset')
    ],
    core: {
        builder: "webpack5",
    },
    webpackFinal: (config) => {
        /**
         * Add support for absolute imports
         */
        config.resolve.plugins = [
            ...(config.resolve.plugins || []),
            new TsconfigPathsPlugin({
                configFile: path.resolve(__dirname, '../storybook.tsconfig.json'),
            }),
        ];

        /**
         * Fixes font import with /
         * @see https://github.com/storybookjs/storybook/issues/12844#issuecomment-867544160
         */
        config.resolve.roots = [
            path.resolve(__dirname, '../public'),
            'node_modules',
        ];

        /**
         * Fixes build issues
         * @see https://github.com/storybookjs/storybook/issues/16690#issuecomment-971579785
         */
        config.module.rules.push({
            test: /\.mjs$/,
            include: /node_modules/,
            type: "javascript/auto",
        })

        /**
         * Load SCSS into stories
         */        

        config.module.rules.push({
            test: /\.scss$/,
            use: [
                'style-loader', 
                'css-loader',
                'postcss-loader',
                {
                    loader: 'sass-loader',
                    options: {
                      // Prefer `dart-sass`
                      implementation: require("sass"),
                    },
                  }
            ],
            include: path.resolve(__dirname, '../'),
        });

        return config;
    },
};
