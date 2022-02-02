const
    fs = require('fs'),
    gulp = require('gulp'),
    path = require("path"),
    svgSprite = require('gulp-svg-sprites'),
    favicons = require('gulp-favicons'),
    childProcess = require('child_process');

const uiPath = 'vNext.App/UI';
const uiAssetsDistPath = `vNext.App/public`;

// Compress image assets and copy to /dist
const images = () => {

    return gulp
        .src(`${uiPath}/images/**/*`)
        .pipe(gulp.dest(`${uiAssetsDistPath}/images`));

};

// Copy fonts from src to dist folder
const fonts = () => {

    return gulp
        .src(`${uiPath}/fonts/**/*`)
        .pipe(gulp.dest(`${uiAssetsDistPath}/fonts`));

};

// Copy tinymce files from node_modules to dist folder
const tinyMce = () => {

    return gulp
        .src('./node_modules/tinymce/**/*')
        .pipe(gulp.dest(`${uiAssetsDistPath}/js/tinymce`));

};

// Generate favicon set
const favicon = () => {

    return gulp.src(`${uiPath}/favicon/logo.png`)
        .pipe(favicons({
            appName: 'Future NHS',
            appDescription: '',
            developerName: '',
            developerURL: '',
            background: '#fff',
            theme_color: '#fff',
            path: '',
            url: '/',
            display: 'standalone',
            orientation: 'any',
            start_url: '/',
            version: 1.0,
            logging: false,
            html: 'index.html',
            pipeHTML: true,
            replace: true,
            icons : {
                appleStartup: false,
                firefox: false,
                windows: false,
                yandex: false,
                coast: false
            }
        }))
        .pipe(gulp.dest(`${uiAssetsDistPath}/favicon`));

};

// Generate svg 'sprite'
const icons = () => {

    const src = `${uiPath}/icons/**/*.svg`;
    const dist = `${uiAssetsDistPath}/icons`;

    const sprite = gulp.src(src)
        .pipe(svgSprite({
            mode: 'symbols',
            common: 'c-svg-icon',
            svgId: 'icon-%f',
            preview: {
                sprite: 'icons.html',
                defs: 'defs.html',
                symbols: 'icons.html',
            },
            svg: {
                sprite: 'icons.svg',
                defs: 'defs.svg',
                symbols: 'icons.svg',
            },
            templates: {
                symbols: fs.readFileSync('./vNext.App/svgSymbolsTemplate.svg', 'utf-8')
            }
        }))
        .pipe(gulp.dest(dist));

    return sprite;

};

// Build task - runs all the web tasks
const build = gulp.series(images, icons, fonts, tinyMce, favicon);


const startSite = (done) => {

    const proc = childProcess.spawn('node', [
        '../node_modules/pm2/bin/pm2',
        'start',
        'node',
        '--name=nhs.futures.app',
        '--',
        'node_modules/cross-env/src/bin/cross-env.js',
        'NODE_ENV=development',
        'node',
        'server.js'
    ], {
        cwd: path.join(process.cwd(), 'vNext.App')
    });

    const re = /SCS\d{4}/;
    proc.stdout.on("data", (data) => {
        console.log(data.toString());

        const match = re.exec(data.toString());
        if (match) {
            done(new Error('Security warning found when building project'));
        }
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error compiling project'));
        }

        return done();
    });
};

const stopSite = (done) => {

    const proc = childProcess.spawn('node', [
        './node_modules/pm2/bin/pm2',
        'delete',
        'nhs.futures.app'
    ], {
        cwd: process.cwd()
    });

    proc.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        return done();
    });
};

module.exports = {
    images,
    icons,
    fonts,
    favicon,
    build,
    startSite,
    stopSite
}