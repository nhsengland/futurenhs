const path = require('path');
const withPWA = require('next-pwa');

const assetPrefix = '';

module.exports = withPWA({
    pwa: {
        disable: process.env.NODE_ENV === 'development'
    },
    webpack(config) {

        config.module.rules.push({
            test: /\.svg$/,
            use: ["@svgr/webpack"]
        });

        return config;
        
    },
    pageExtensions: ['page.tsx', 'page.ts', 'page.jsx', 'page.js'],
    sassOptions: {
        includePaths: [path.join(__dirname, '')],
        quietDeps: true
    },
    images: {
        domains: ['localhost', '127.0.0.1', 'sacdsfnhsdevuksouthpub.blob.core.windows.net'],
        path: `${assetPrefix}/_next/image`
    },
    assetPrefix: assetPrefix
});
