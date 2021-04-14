const webpack = require('webpack');
const path = require('path');
const sourcePath = path.join(path.resolve() + '/MVCForum.Website/UI/assets/src/ts/root');
const outPath = path.join(path.resolve() + '/MVCForum.Website/UI/assets/dist/js');
const DuplicatePackageCheckerPlugin = require('duplicate-package-checker-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const { TsConfigPathsPlugin } = require('awesome-typescript-loader');

module.exports = {
    mode: 'production',
    context: sourcePath,
    entry: {
        ['polyfills']: ['idempotent-babel-polyfill', 'whatwg-fetch', 'details-element-polyfill', './polyfills.ts'],
        ['global']: ['./global.ts']
    },
    output: {
        path: outPath,
        publicPath: '/UI/assets/dist/js/',
        chunkFilename: `[name]-[chunkhash]-chunk.js`,
        filename: '[name].min.js'
    },
    target: 'web',
    resolve: {
        extensions: ['.js', '.ts', '.tsx', '.css', '.scss'],
        mainFields: ['main'],
        plugins: [
            new TsConfigPathsPlugin()
        ]
    },
    optimization: {
        minimizer: [new TerserPlugin({
            cache: true,
            parallel: true,
            extractComments: true,
            sourceMap: true
        })]
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'awesome-typescript-loader'
            },
            {
                test: /\.scss$/,
                use: ['css-loader?modules', 'sass-loader'],
            }
        ]
    },
    plugins: [
        new webpack.DefinePlugin({
            __ISDEVENV__: false
        }),
        new CleanWebpackPlugin(),
        new DuplicatePackageCheckerPlugin(),
        new BundleAnalyzerPlugin({
            analyzerMode: 'disabled',
            generateStatsFile: true
        })
    ]
};
