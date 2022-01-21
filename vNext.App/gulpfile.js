const
    fs = require("fs"),
    gulp = require("gulp"),
    svgSprite = require("gulp-svg-sprites"),
    favicons = require("gulp-favicons"),
    plumber = require("gulp-plumber"),
    rename = require("gulp-rename"),
    sass = require("gulp-sass")(require("sass")),
    sassLint = require("gulp-sass-lint"),
    sassThemes = require("gulp-sass-themes"),
    postcss = require("gulp-postcss"),
    tailWind = require("tailwindcss"),
    autoprefixer = require("autoprefixer"),
    pxtorem = require("postcss-pxtorem"),
    tailWindConfig = require('./tailwind.config');

const uiPath = 'UI';
const uiAssetsDistPath = `public`;

// Scss/css minification & bundling
const scss = () => {

    return gulp.src(`${uiPath}/scss/**/*.s+(a|c)ss`)
        .pipe(plumber())
        .pipe(sassLint({
            configFile: 'sass-lint.yml',
            options: {
                formatter: 'stylish'
            }
        }))
        .pipe(sassLint.format())
        .pipe(sassLint.failOnError())
        .pipe(sassThemes(`${uiPath}/scss/theme-variables/**/*.scss`))
        .pipe(sass({
            outputStyle: 'compressed',
            quietDeps: true
        }).on('error', sass.logError))
        .pipe(postcss([
            tailWind(tailWindConfig),
            autoprefixer({}),
            pxtorem({
                rootValue: 16,
                propList: ['font-size', 'line-height'],
                replace: false,
                mediaQuery: false
            })
        ]))
        .pipe(rename({
            suffix: '.min'
        }))
        .pipe(gulp.dest(`${uiAssetsDistPath}/css/`));

};

gulp.task(scss);

// Compress image assets and copy to /dist
const images = () => {

    return gulp
        .src(`${uiPath}/images/**/*`)
        .pipe(gulp.dest(`${uiAssetsDistPath}/images`));

};

gulp.task(images);

// Copy fonts from src to dist folder
const fonts = () => {

    return gulp
        .src(`${uiPath}/fonts/**/*`)
        .pipe(gulp.dest(`${uiAssetsDistPath}/fonts`));

};

gulp.task(fonts);

// Copy tinymce files from node_modules to dist folder
const tinyMce = () => {

    return gulp
        .src('./node_modules/tinymce/**/*')
        .pipe(gulp.dest(`${uiAssetsDistPath}/js/tinymce`));

};

gulp.task(tinyMce);

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

gulp.task(favicon);

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
                symbols: fs.readFileSync('./svgSymbolsTemplate.svg', 'utf-8')
            }
        }))
        .pipe(gulp.dest(dist));

    return sprite;

};

gulp.task(icons);

// Build task - runs all the web tasks
const build = (done) => { 

    gulp.series(scss, images, icons, fonts, tinyMce, favicon)();

    done();

};

gulp.task(build);

// Watch task - runs all the web tasks then watches and re-runs tasks on subsequent changes - also hosts local prototyping server for prototyping
const watch = (done) => { 

    const watchers = () => {

        gulp.watch(`${uiPath}/scss/**/*.scss`, gulp.series(scss));
        gulp.watch([`${uiPath}/images/**/*`], gulp.series(images));
        gulp.watch([`${uiPath}/icons/**/*`], gulp.series(icons));
        gulp.watch([`${uiPath}/favicon/**/*`], gulp.series(favicon));
        gulp.watch([`${uiPath}/fonts/**/*`], gulp.series(fonts));

    };

    gulp.series(build, watchers)();

    done();

};

gulp.task(watch);
