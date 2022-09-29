const fs = require('fs'),
    gulp = require('gulp'),
    path = require('path'),
    svgSprite = require('gulp-svg-sprites'),
    childProcess = require('child_process')

const getRootPath = () =>
    process.cwd().includes('futurenhs.app') ? '' : 'futurenhs.app'
const getUiPath = () => (getRootPath() ? `${getRootPath()}/assets` : './assets')
const getUiAssetsDistPath = () =>
    getRootPath() ? `${getRootPath()}/public` : './public'

// Compress image assets and copy to /dist
const images = () => {
    return gulp
        .src(`${getUiPath()}/images/**/*`)
        .pipe(gulp.dest(`${getUiAssetsDistPath()}/images`))
}

// Copy tinymce files from node_modules to dist folder
const tinyMce = () => {
    return gulp
        .src('./node_modules/tinymce/**/*')
        .pipe(gulp.dest(`${getUiAssetsDistPath()}/js/tinymce`))
}

// Copy favicons to dist folder
const favicon = () => {
    return gulp
        .src(`${getUiPath()}/favicon/**/*`)
        .pipe(gulp.dest(`${getUiAssetsDistPath()}/favicon`))
}

// Generate svg 'sprite'
const icons = () => {
    const src = `${getUiPath()}/icons/**/*.svg`
    const dist = `${getUiAssetsDistPath()}/icons`
    const templatePath = getRootPath()
        ? `./${getRootPath()}/svgSymbolsTemplate.svg`
        : './svgSymbolsTemplate.svg'

    const sprite = gulp
        .src(src)
        .pipe(
            svgSprite({
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
                    symbols: fs.readFileSync(templatePath, 'utf-8'),
                },
            })
        )
        .pipe(gulp.dest(dist))

    return sprite
}

// Build task - runs all the web tasks
const build = gulp.series(images, icons, tinyMce, favicon)

const startSite = (done) => {
    const proc = childProcess.spawn(
        'node',
        [
            '../node_modules/pm2/bin/pm2',
            'start',
            'node',
            '--name=futurenhs.app',
            '--',
            'node_modules/cross-env/src/bin/cross-env.js',
            'NODE_ENV=development',
            'node',
            'server.js',
        ],
        {
            cwd: path.join(process.cwd(), 'futurenhs.app'),
        }
    )

    const re = /SCS\d{4}/
    proc.stdout.on('data', (data) => {
        console.log(data.toString())

        const match = re.exec(data.toString())
        if (match) {
            done(new Error('Security warning found when building project'))
        }
    })

    proc.stderr.on('data', (data) => {
        console.log(data.toString())
    })

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error compiling project'))
        }

        return done()
    })
}

const stopSite = (done) => {
    const proc = childProcess.spawn(
        'node',
        ['./node_modules/pm2/bin/pm2', 'delete', 'futurenhs.app'],
        {
            cwd: process.cwd(),
        }
    )

    proc.stdout.on('data', (data) => {
        console.log(data.toString())
    })

    proc.stderr.on('data', (data) => {
        console.log(data.toString())
    })

    proc.on('close', (code) => {
        return done()
    })
}

module.exports = {
    images,
    icons,
    favicon,
    build,
    startSite,
    stopSite,
}
