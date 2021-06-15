const
    fs = require("fs"),
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
    jestConfig = require("./jest.config.js"),
    { AxePuppeteer } = require('@axe-core/puppeteer'),
    puppeteer = require('puppeteer');

////////////////////////////////////////////////////////////////////////
// APP BUILD TASKS
////////////////////////////////////////////////////////////////////////
const uiPath = 'MVCForum.Website/UI';
const uiAssetsSrcPath = `${uiPath}/assets/src`;
const uiAssetsDistPath = `${uiPath}/assets/dist`;
const uiProtoTypesSrcPath = `${uiPath}/prototypes/src`;
const uiProtoTypesDistPath = `${uiPath}/prototypes/dist`;

// Empties set folders in /dist
const clean = () => {

    return del([
        `${uiAssetsDistPath}/**/*`
    ]);

};

gulp.task(clean);

// Build .net solution
const msbuild = (done) => {

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
};

gulp.task(msbuild);

// Generic back end build
const build = (done) => {
    
    gulp.series(msbuild)();
    done();

}

gulp.task(build);

// Start the site
const startSite = (done) => {

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

};

gulp.task(startSite);

// Stop the site
const stopSite = (done) => {

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

};

gulp.task(stopSite);

// Activate
const activate = (done) => {

    gulp.series(stopSite, build, buildWeb, startSite)();

    done();

};

gulp.task(activate);

// Deactivate
const deactivate = (done) => {

    gulp.series(stopSite)();

    done();

};

gulp.task(deactivate);

// End process
const terminate = (done) => {

    console.log("terminating...");
    done();
    process.exit(0);

};

gulp.task(terminate);

////////////////////////////////////////////////////////////////////////
// UI BUILD TASKS
////////////////////////////////////////////////////////////////////////

// Scss/css minification & bundling
const scss = (done) => {

    const postcssOpts = { // any options to supply to postcss plugins
        autoprefixer: {},
        pxtorem: {
            // defines the root px value
            rootValue: 16,
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
        .pipe(gulp.dest(`${uiAssetsDistPath}/css/`)); // and put the compiled css files in the dist folder

};

gulp.task(scss);

// Copy fonts from src to dist folder
const fonts = () => {

    return gulp
        .src(`${uiAssetsSrcPath}/fonts/**/*`)
        .pipe(gulp.dest(`${uiAssetsDistPath}/fonts`));

};

gulp.task(fonts);

// JavaScript
const js = () => {

    return gulp.src([`${uiAssetsSrcPath}/ts/root/*.ts`])
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError())
        .pipe(webpackStream(webpackConfig, webpack))
        .pipe(gulp.dest(`${uiAssetsDistPath}/js`));

};

gulp.task(js);

// JavaScript unit tests
const testJs = () => {

    return gulp.src([`${uiAssetsSrcPath}/ts/root/*`])
        .pipe(jest(jestConfig))

};

gulp.task(testJs);

// Service worker
const sw = () => {

    return workbox.generateSW(
        workBoxCliConfig
    ).then(() => {

        console.info('Service worker generation completed.');

    }).catch((error) => {

        console.warn('Service worker generation failed: ' + error);

    });

};

gulp.task(sw);

// Html prototype template building using nunjucks: https://mozilla.github.io/nunjucks/
const templates = () => {

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

};

gulp.task(templates);

// Compress image assets and copy to /dist
const images = () => {

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
        .pipe(gulp.dest(`${uiAssetsDistPath}/img/`));

};

gulp.task(images);

// Generate favicon set
const favicon = () => {

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

};

gulp.task(favicon);

// Generate svg 'sprite'
const initSvgSprite = () => {

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

};

gulp.task(initSvgSprite);

// Start browserSync server
const initBrowserSync = () => {

    browserSync({
        server: {
            port: 3000,
            baseDir: path.join(process.cwd()),
            directory: true
        },
        startPath: `${uiProtoTypesDistPath}/index.html`
    });

};

gulp.task(initBrowserSync);

// Front end accessibility testing
const testAxe = (done) => {

    const testURLs = require('./axeReports/testURLs');
    const LOGIN_PAGE_URL = 'http://localhost:8888/members/logon/'; 
    
    let totalViolations = 0;

    const getAxeReport = async (testURL) => {

        const browser = await puppeteer.launch({
            headless: true,
            defaultViewport: null,
        });

        //creates login session
        const loginPage = await browser.newPage();
        await loginPage.setBypassCSP(true);
        await loginPage.goto(LOGIN_PAGE_URL);

        //gets username and passwords fields
        const userNameInput = await loginPage.$("#UserName");
        const passwordInput = await loginPage.$("#Password");
        
        //types credentials
        await userNameInput.type("admin");
        await passwordInput.type("ks2qlh89-mvc");

        //submits form 
        loginPage.$eval('.form-login', form => form.submit());
        
        //waits 1.5s , gives time to login session to complete before starting running reports  
        await loginPage.waitForTimeout(1500);
        await loginPage.close();

        //test url after login
        const testPage = await browser.newPage();
        await testPage.setBypassCSP(true);
        await testPage.goto(testURL);

        //analyze url and trim result data
        const rawReportResults = await new AxePuppeteer(testPage).analyze();       
        const { violations, passes, timestamp, url } = rawReportResults;
        const violationsCount = violations.length;
        const passesCount = passes.length;
        const reportResults = {
            [url]: {    
                timestamp,
                passesCount,
                violationsCount,
                ...rawReportResults
            }
        };

        //track violations count
        totalViolations += violationsCount; 
        
        await testPage.close();
        await browser.close();

        return reportResults;

    };

    return Promise.all(testURLs.map((testURL) => {

        return getAxeReport(testURL);

    })).then((reportResults)=>{

        fs.writeFileSync(
            "./axeReports/reportResults.json",
            JSON.stringify(reportResults, null, "  ")
        );

        if(Boolean(totalViolations)){
            throw new Error('Accessbility issues found');
        }

        done();
        return reportResults;

    }).catch((error)=>{

        console.error('Error: ' + error.message);
        
        done(new Error(`The accessibility testing has finished with ${totalViolations} violations`));
        process.exit(-1);

    }).finally(()=>{
       
        console.log(`The accessibility testing has finished with 0 violations`);
        done();

    });

};

gulp.task(testAxe);


////////////////////////////////////////////////////////////////////////
// CALLING UI TASKS
////////////////////////////////////////////////////////////////////////

// Build task - runs all the web tasks
const buildWeb = (done) => { 

    gulp.series(clean, initSvgSprite, templates, scss, js, fonts, images, favicon, sw)();

    done();

};

gulp.task(buildWeb);

// Watch task - runs all the web tasks then watches and re-runs tasks on subsequent changes - also hosts local prototyping server for prototyping
const watchWeb = (done) => { 

    gulp.series(buildWeb, watchBasic)();

    done();

};

gulp.task(watchWeb);

// Basic watch task - watches and re-runs tasks on subsequent changes
const watchBasic = () => {

    // gulp.watch(`${uiProtoTypesDistPath}/*.html`, browserSync.reload);
    gulp.watch(`${uiAssetsSrcPath}/**/*.scss`, gulp.series(scss));
    gulp.watch([`${uiProtoTypesSrcPath}/pages/*.html`, `${uiProtoTypesSrcPath}/layouts/*.html`, `${uiProtoTypesSrcPath}/partials/**/*.html`], gulp.series(templates, browserSync.reload));
    gulp.watch([`${uiAssetsSrcPath}/ts/**/*.ts`], gulp.series(js));
    gulp.watch([`${uiAssetsSrcPath}/img/**/*`, `${uiAssetsSrcPath}/img/*`, `!${uiAssetsSrcPath}/img/{sprite,sprite/**/*,}`, `!${uiAssetsSrcPath}/img/favicon/**/*,}`], gulp.series(images));
    gulp.watch([`${uiAssetsSrcPath}/img/svg-icons/active/*`], gulp.series(favicon));
    gulp.watch('./workbox.cli.config.js', gulp.series(sw));

};

gulp.task(watchBasic);

// Run tests
const test = (done) => {

    gulp.series(testJs, testAxe)();

    done();

};

gulp.task(test);
