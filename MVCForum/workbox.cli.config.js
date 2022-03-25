module.exports = {
    globDirectory: 'MVCForum/MVCForum.Website',
    globPatterns: [
        'UI/assets/dist/js/*',
        'UI/assets/dist/css/*',
        'UI/assets/dist/images/content/*',
        'UI/assets/dist/fonts/*'
    ],
    swDest: 'MVCForum/MVCForum.Website/sw.js',
    clientsClaim: true,
    skipWaiting: true,
    runtimeCaching: []
};