const path = require('path');
const withPWA = require('next-pwa');

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
        includePaths: [path.join(__dirname, 'UI/assets/src/scss')],
        quietDeps: true
    },
    images: {
        domains: ['localhost', 'picsum.photos'],
    }
});