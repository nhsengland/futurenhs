const path = require('path');
const withPlugins = require('next-compose-plugins');
const withPWA = require('next-pwa');
const withBundleAnalyzer = require('@next/bundle-analyzer')({
    enabled: process.env.ANALYZE === 'true',
  });
const swRunTimeCachingConfig = require('./sw.cache.config');

const assetPrefix = '';

const baseConfig = {
    pwa: {
        disable: process.env.NODE_ENV === 'development',
        runtimeCaching: swRunTimeCachingConfig,
        buildExcludes: [/middleware-manifest.json$/]
    },
    webpack(config) {

        config.module.rules.push({
            test: /\.svg$/,
            use: ['@svgr/webpack']
        });

        return config;
        
    },
    pageExtensions: ['page.tsx', 'page.ts', 'page.jsx', 'page.js'],
    sassOptions: {
        includePaths: [path.join(__dirname, '')],
        quietDeps: true
    },
    images: {
        domains: [
            'localhost', 
            '127.0.0.1', 
            'timblobtest.blob.core.windows.net',
            'collaborate-dev.future.nhs.uk',
            'collaborate-uat.future.nhs.uk',
            'collaborate.future.nhs.uk'
        ],
        path: `${assetPrefix}/_next/image`
    },
    assetPrefix: assetPrefix
};

module.exports = withPlugins([
    [withPWA, {
        pwa: {
            disable: process.env.NODE_ENV === 'development',
            runtimeCaching: swRunTimeCachingConfig,
            buildExcludes: [/middleware-manifest.json$/]
        }
    }],
    [withBundleAnalyzer]
], baseConfig);
