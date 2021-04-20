const
    fs = require("fs"),
    sequence = require("gulp-sequence"),
    childProcess = require("child_process"),
    path = require("path"),
    gulp = require("gulp"),
    del = require('del'),
    sass = require("gulp-sass"),
    sassThemes = require("gulp-sass-themes"),
    rename = require("gulp-rename"),
    nunjucksRender = require("gulp-nunjucks-render"),
    imagemin = require("gulp-imagemin"),
    pngquant = require("imagemin-pngquant"),
    browserSync = require("browser-sync"),
    postcss = require("gulp-postcss"),
    autoprefixer = require("autoprefixer"),
    pxtorem = require("postcss-pxtorem"),
    sassLint = require("gulp-sass-lint"),
    merge = require("merge-stream"),
    plumber = require("gulp-plumber"),
    eslint = require('gulp-eslint'),
    svgSprite = require("gulp-svg-sprites"),
    favicons = require("gulp-favicons"),
    webpackStream = require("webpack-stream"),
    webpack = require("webpack"),
    workbox = require("workbox-build"),
    jest = require("gulp-jest").default,
    webpackConfig = require("./webpack.config.js"),
    workBoxCliConfig = require("./workbox.cli.config.js"),
    jestConfig = require("./jest.config.js");

const sequenceArgs = (list) => {
    return (done) => {
        const doNext = () => {
            if (list.length === 0) {
                return Promise.resolve();
            }

            const next = list.shift();

            return new Promise((resolve, reject) => {
                const proc = childProcess.spawn('node', [
                    './node_modules/gulp/bin/gulp.js'
                ].concat(next), {
                    cwd: process.cwd()
                });

                proc.stdout.on('data', (data) => {
                    console.log(data.toString().replace(/\n$/, ''));
                });

                proc.stderr.on('data', (data) => {
                    console.log(data.toString().replace(/\n$/, ''));
                });

                proc.on('close', (code) => {
                    if (code !== 0) {
                        reject(new Error('None zero error code returned'));
                        process.exit(code);
                        return;
                    }

                    return resolve();
                });
            }).then(doNext);
        };

        doNext().then(done).catch(done);
    };
}

////////////////////////////////////////////////////////////////////////
// APP BUILD TASKS
////////////////////////////////////////////////////////////////////////
const uiPath = 'MVCForum.Website/UI';
const uiAssetsSrcPath = `${uiPath}/assets/src`;
const uiAssetsDistPath = `${uiPath}/assets/dist`;
const uiProtoTypesSrcPath = `${uiPath}/prototypes/src`;
const uiProtoTypesDistPath = `${uiPath}/prototypes/dist`;


// Empties set folders in /dist
gulp.task('clean', () => {

    return del([
        `${uiAssetsDistPath}/**/*`
    ]);

});

gulp.task('nuget:restore', (done) => {
    const proc = childProcess.spawn('./.nuget/nuget.exe', [
        'restore',
        '-source',
        'https://api.nuget.org/v3/index.json'
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
        if (code !== 0) {
            return done(new Error('Error carrying out nuget restore'));
        }

        return done();
    });
});

gulp.task('msbuild', (done) => {

    process.env.PATH = `${process.env.PATH};C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\MSBuild\\Current\\Bin`;

    const proc = childProcess.spawn('msbuild.exe', [
        'MVCForum.sln',
        '-t:rebuild'
    ], {
        cwd: process.cwd()
    });

    const re = /SCS\d{4}/;
    proc.stdout.on('data', (data) => {
        console.log(data.toString());

        const match = re.exec(data.toString());
        if (match) {
            return done(new Error('Security warning found when building project'));
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
});

gulp.task('build', sequence(
    //'nuget:restore', // TODO: Error - The request was aborted: Could not create SSL/TLS secure channel
    'msbuild'
));

gulp.task('build:nuget', (done) => {
    const package = JSON.parse(fs.readFileSync('package.json'));

    return sequenceArgs([
        ['nuget:package', `--version=${package.version}`]
    ])(done);
});

gulp.task('start:site', (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\IIS Express`;

    const proc = childProcess.spawn('node', [
        './node_modules/pm2/bin/pm2',
        'start',
        'iisexpress.exe',
        '--name=nhs.futures.website',
        '--',
        '-port:8888',
        `-path:${path.join(process.cwd(), 'MVCForum.website')}`
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
        if (code !== 0) {
            return done(new Error('None zero error code returned when attempting to start MVCForum.website'));
        }

        return done();
    });
});

gulp.task('stop:site', (done) => {

    const proc2 = childProcess.spawn('node', [
        './node_modules/pm2/bin/pm2',
        'delete',
        'nhs.futures.website'
    ], {
        cwd: process.cwd()
    });

    proc2.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    proc2.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc2.on('close', (code) => {
        return done();
    });
});

gulp.task('activate', sequence(
    //'db:build', <-- TODO how are we going to handle the DB?
    'build',
    'build:web',
    'stop:site',
    'start:site'
));

gulp.task('deactivate', sequence(
    'stop:site',
    //'db:remove' <-- TODO how are we going to handle the DB?
));

gulp.task('test', sequence(
    'test:accessibility',
    'test:html',
    'test:js',
    'terminate'
));

gulp.task('terminate', (done) => {
    console.log("terminating...");
    process.exit(0);
});

////////////////////////////////////////////////////////////////////////
// UI BUILD TASKS
////////////////////////////////////////////////////////////////////////

// Scss/css minification & bundling
gulp.task('scss', () => {

    const postcssOpts = { // any options to supply to postcss plugins
        autoprefixer: {},
        pxtorem: {
            // defines the root px value
            rootValue: 14,
            // The properties that can change from px to rem. * to enable all properties. ! to not match a property
            propList: ['font-size', 'line-height'],
            // does not replace px, but adds rems after
            replace: false,
            // allow px to be converted in media queries
            mediaQuery: false,
        }
    };

    const postcssProcessors = [ // postcss plugins to run on compiled stylesheets
        autoprefixer(postcssOpts.autoprefixer), // add any required browser prefixes
        pxtorem(postcssOpts.pxtorem) // convert px units on certain selctors to rems
    ];

    return gulp.src(`${uiAssetsSrcPath}/scss/**/*.s+(a|c)ss`)
        .pipe(plumber()) // catch errors without crashing gulp
        .pipe(sassLint({
            configFile: 'sass-lint.yml',
            options: {
                formatter: 'stylish'
            },
        }))
        .pipe(sassLint.format())
        .pipe(sassLint.failOnError())
        .pipe(sassThemes(`${uiAssetsSrcPath}/scss/theme-variables/**/*.scss`))
        .pipe(sass({
            outputStyle: 'compressed'
        }).on('error', sass.logError)) // compile css
        .pipe(postcss(postcssProcessors)) // run postcss tasks
        .pipe(rename({
            suffix: '.min'
        })) // and filename suffix
        .pipe(gulp.dest(`${uiAssetsDistPath}/css/`)) // and put the compiled css files in the dist folder
        .pipe(browserSync.reload({ // reload any browsers displaying prototypes
            stream: true
        }));

});

// Copy fonts from src to dist folder
gulp.task('fonts', () => {

    return gulp
        .src(`${uiAssetsSrcPath}/fonts/**/*`)
        .pipe(gulp.dest(`${uiAssetsDistPath}/fonts`));

});

// JavaScript
gulp.task('js', () => {

    return gulp.src([`${uiAssetsSrcPath}/ts/root/*.ts`])
        .pipe(plumber())
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError())
        .pipe(webpackStream(webpackConfig, webpack))
        .pipe(gulp.dest(`${uiAssetsDistPath}/js`));

});

// JavaScript unit tests
gulp.task('test:js', () => {

    return gulp.src([`${uiAssetsSrcPath}/ts/root/*`])
        .pipe(jest(jestConfig));

});

// Service worker
gulp.task('sw', () => {

    return workbox.generateSW(
        workBoxCliConfig
    ).then(() => {

        console.info('Service worker generation completed.');

    }).catch((error) => {

        console.warn('Service worker generation failed: ' + error);

    });

});

// Html prototype template building using nunjucks: https://mozilla.github.io/nunjucks/
gulp.task('templates', () => {

    nunjucksRender.nunjucks.configure([`${uiProtoTypesSrcPath}/pages/`], {
        watch: false
    });

    return gulp.src(`${uiProtoTypesSrcPath}/pages/**/!(_)*.html`)
        .pipe(plumber())
        .pipe(nunjucksRender({
            css_path: `${uiAssetsDistPath}/css/`,
            js_path: `${uiAssetsDistPath}/js/`,
            img_path: `${uiAssetsDistPath}/images/`
        }))
        .pipe(gulp.dest(`${uiProtoTypesDistPath}`));

});

// Compress image assets and copy to /dist
gulp.task('images', () => {

    // all in images folder except sprite icons
    return gulp.src([`${uiAssetsSrcPath}/img/**/*`, `!${uiAssetsSrcPath}/img/svg-sprite/**/*}`])
        .pipe(plumber())
        .pipe(imagemin({
            progressive: true,
            svgoPlugins: [{
                removeViewBox: false
            },
            {
                cleanupIDs: false
            }
            ],
            use: [pngquant()]
        }))
        .pipe(gulp.dest(`${uiAssetsDistPath}/img/`))
        .pipe(browserSync.reload({
            stream: true
        }));

});

// Generate favicon set
gulp.task('favicon', () => {

    return gulp.src(`${uiAssetsSrcPath}/img/favicon/logo.png`)
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
        .pipe(gulp.dest(`${uiAssetsDistPath}/img/favicon`));

});

// Generate svg 'sprite'
gulp.task('svgSprite', () => {

    const uiSrc = `${uiAssetsSrcPath}/img/svg-sprite/active/ui/**/*.svg`;
    const contentSrc = `${uiAssetsSrcPath}/img/svg-sprite/active/content/**/*.svg`;
    const dest = `${uiAssetsDistPath}/img/svg-sprite`;

    const uiSvg = gulp.src(uiSrc)
        .pipe(svgSprite({
            mode: 'symbols',
            common: 'c-svg-icon',
            svgId: 'icon-%f',
            preview: {
                sprite: 'ui/sprite.html',
                defs: 'ui/defs.html',
                symbols: 'ui/symbols.html',
            },
            svg: {
                sprite: 'ui/sprite.svg',
                defs: 'ui/defs.svg',
                symbols: 'ui/symbols.svg',
            },
            templates: {
                symbols: fs.readFileSync('./svgSymbolsTemplate.svg', 'utf-8')
            }
        }))
        .pipe(gulp.dest(dest));

    const contentSvg = gulp.src(contentSrc)
        .pipe(svgSprite({
            mode: 'symbols',
            common: 'c-svg-icon',
            svgId: 'icon-%f',
            preview: {
                sprite: 'content/sprite.html',
                defs: 'content/defs.html',
                symbols: 'content/symbols.html',
            },
            svg: {
                sprite: 'content/sprite.svg',
                defs: 'content/defs.svg',
                symbols: 'content/symbols.svg',
            },
            templates: {
                symbols: fs.readFileSync('./svgSymbolsTemplate.svg', 'utf-8')
            }
        }))
        .pipe(gulp.dest(dest));

    const contentSvgLeft = gulp.src(contentSrc)
        .pipe(svgSprite({
            mode: 'symbols',
            common: 'c-svg-icon',
            svgId: 'icon-%f',
            preview: null,
            svg: {
                sprite: 'content/sprite--left.svg',
                defs: 'content/defs--left.svg',
                symbols: 'content/symbols--left.svg',
            },
            templates: {
                symbols: fs.readFileSync('./svgSymbolsTemplateLeft.svg', 'utf-8')
            }
        }))
        .pipe(gulp.dest(dest));

    const contentSvgRight = gulp.src(contentSrc)
        .pipe(svgSprite({
            mode: 'symbols',
            common: 'c-svg-icon',
            svgId: 'icon-%f',
            preview: null,
            svg: {
                sprite: 'content/sprite--right.svg',
                defs: 'content/defs--right.svg',
                symbols: 'content/symbols--right.svg',
            },
            templates: {
                symbols: fs.readFileSync('./svgSymbolsTemplateRight.svg', 'utf-8')
            }
        }))
        .pipe(gulp.dest(dest));

    return merge(uiSvg, contentSvg, contentSvgLeft, contentSvgRight);

});

// Start browserSync server
gulp.task('browserSync', () => {

    browserSync({
        server: {
            port: 3000,
            baseDir: path.join(process.cwd()),
            directory: true
        },
        startPath: `${uiProtoTypesDistPath}/index.html`
    });

});

////////////////////////////////////////////////////////////////////////
// CALLING UI TASKS
////////////////////////////////////////////////////////////////////////

// Build task - runs all the web tasks
gulp.task('build:web', sequence(
    'clean',
    'svgSprite',
    'templates',
    'fonts',
    'images',
    'favicon',
    'scss',
    'js',
    'sw'
));

// Watch task - runs all the web tasks then watches and re-runs tasks on subsequent changes - also hosts local prototyping server for prototyping
gulp.task('watch:web', ['build:web', 'watch:basic', 'browserSync']);

// Basic watch task - watches and re-runs tasks on subsequent changes
gulp.task('watch:basic', () => {

    gulp.watch(`${uiProtoTypesDistPath}/*.html`, browserSync.reload);
    gulp.watch(`${uiAssetsSrcPath}/**/*.scss`, ['scss']);
    gulp.watch([`${uiProtoTypesSrcPath}/pages/*.html`, `${uiProtoTypesSrcPath}/layouts/*.html`, `${uiProtoTypesSrcPath}/partials/**/*.html`], ['templates', browserSync.reload]);
    gulp.watch([`${uiAssetsSrcPath}/ts/**/*.ts`], ['js']);
    gulp.watch([`${uiAssetsSrcPath}/img/**/*`, `${uiAssetsSrcPath}/img/*`, `!${uiAssetsSrcPath}/img/{sprite,sprite/**/*,}`, `!${uiAssetsSrcPath}/img/favicon/**/*,}`], ['images']);
    gulp.watch([`${uiAssetsSrcPath}/img/svg-icons/active/*`], ['svgSprite']);
    gulp.watch('./workbox.cli.config.js', ['sw']);

});
