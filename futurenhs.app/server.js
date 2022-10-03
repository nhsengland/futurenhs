const isProduction = process.env.NODE_ENV === 'production'
const isDevelopment = process.env.NODE_ENV === 'development'

/**
 * Import dependencies
 */
const express = require('express')
const proxy = require('express-http-proxy')
const next = require('next')
const url = require('url')
const { join } = require('path')
const csrf = require('csurf')
const cookieParser = require('cookie-parser')
const formData = require('express-form-data')
const os = require('os')
const { randomBytes } = require('crypto')
const { AbortController } = require('node-abort-controller')

const csrfExcludedPaths = [
    '/api/auth/csrf',
    '/api/auth/signin',
    '/api/auth/signout',
    '/api/auth/signout/azure-ad-b2c',
    '/api/auth/signin/azure-ad-b2c',
]
const shouldExcludePathFromCsrf = (path) => csrfExcludedPaths.includes(path)

/**
 * Generate Content Security Policy settings
 */
const generateNonce = () => randomBytes(16).toString('base64')
const generateCSP = (nonce) => {
    /**
     * Settings primarily built around an existing GTM setup
     * script-src: unsafe-inline is required to allow the GTM script to load
     * counter-intuitive sources list in connect-src (e.g. fonts) are required for the service worker which returns assets from a cache
     */
    const csp = {
        'frame-src': `'self' https://collaborate.future.nhs.uk https://vars.hotjar.com`,
        'img-src': `'self' data: *.google-analytics.com https://data.eu.pendo.io`,
        'style-src': `'self' 'unsafe-inline'`,
        'script-src': `'self' 'unsafe-inline' https://js.monitor.azure.com/scripts/b/ai.2.min.js *.googletagmanager.com *.hotjar.com https://ws2.hotjar.com wss://*.hotjar.com/api/v2/client/ws *.google-analytics.com https://cdn.eu.pendo.io https://data.eu.pendo.io`,
        'font-src': `'unsafe-inline' https://assets.nhs.uk`,
        'connect-src': `'self' https://dc.services.visualstudio.com *.google-analytics.com *.hotjar.com https://ws2.hotjar.com wss://*.hotjar.com/api/v2/client/ws *.googletagmanager.com https://assets.nhs.uk https://cdn.eu.pendo.io https://data.eu.pendo.io`,
        'worker-src': `'self'`,
    }

    if (isDevelopment) {
        csp[
            'script-src'
        ] = `'self' 'unsafe-inline' 'unsafe-eval' https://js.monitor.azure.com/scripts/b/ai.2.min.js *.googletagmanager.com *.hotjar.com *.google-analytics.com https://cdn.eu.pendo.io https://data.eu.pendo.io`
    }

    return Object.entries(csp).reduce(
        (acc, [key, val]) => `${acc} ${key} ${val};`,
        ''
    )
}

/**
 * Create an Express app instance
 */
const app = express()

let server = undefined

/**
 * Bind the API gateway proxy before other middleware to prevent the original request being mutated
 * This gateway accepts front and back end calls from application services and injects the required auth header
 * before forwarding the request to the API and subsequently the response back to the service
 */
app.use(
    '/api/gateway/*',
    proxy(() => process.env.NEXT_PUBLIC_API_BASE_URL.replace('/api', ''), {
        memoizeHost: false,
        limit: '250mb',
        proxyReqPathResolver: (req) =>
            '/api' + req.originalUrl.split(`gateway`)[1],
        proxyReqOptDecorator: (proxyReqOpts) => {
            proxyReqOpts.headers[
                'Authorization'
            ] = `Bearer ${process.env.SHAREDSECRETS_APIAPPLICATION}`

            return proxyReqOpts
        },
    })
)

/**
 * Bind remaining middleware
 */
app.use(express.json())
app.use(
    formData.parse({
        uploadDir: os.tmpdir(),
        autoClean: true,
    })
)
app.use(formData.format())
app.use(formData.stream())
app.use(formData.union())
app.use(cookieParser(process.env.COOKIE_PARSER_SECRET))

/**
 * Create a Next.js app instance
 */
const nextApp = next({
    dev: isDevelopment,
})
const handle = nextApp.getRequestHandler()

/**
 * Initialise Next
 */
nextApp.prepare().then(() => {
    let appInsightsClient

    /**
     * Start application insights before other dependencies are imported
     */
    if (isProduction && process.env.APPINSIGHTS_INSTRUMENTATIONKEY) {
        appInsightsClient = require('applicationinsights')
        appInsightsClient
            .setup(process.env.APPINSIGHTS_INSTRUMENTATIONKEY)
            .start()
    }

    /**
     * Create a CSRF handler
     */
    const csrfProtection = csrf({
        cookie: {
            secure: true,
        },
    })

    /**
     * Create a custom CSRF handler which excludes certain routes e.g. next-auth routes handle CSRF themselves
     */
    const conditionalCsrf = (req, res, next) => {
        const shouldExclude = shouldExcludePathFromCsrf(req.path)

        if (shouldExclude) {
            return next()
        }

        csrfProtection(req, res, next)
    }

    /**
     * Set response headers
     */
    app.use((req, res, next) => {
        const nonce = generateNonce()

        res.set({
            'Cache-Control': 'no-store',
            'X-DNS-Prefetch-Control': 'on',
            'Strict-Transport-Security':
                'max-age=63072000; includeSubDomains; preload',
            'X-XSS-Protection': '1; mode=block',
            'X-Frame-Options': 'SAMEORIGIN',
            'X-Content-Type-Options': 'nosniff',
            'Referrer-Policy': 'origin-when-cross-origin',
            'Content-Security-Policy': generateCSP(nonce),
        })

        res.locals.nonce = nonce

        next()
    })

    /**
     * Handle GET requests
     */
    app.get('*', conditionalCsrf, (req, res) => {
        try {
            if (!shouldExcludePathFromCsrf(req.path)) {
                const token = req.csrfToken()

                /**
                 * Set CSRF cookie
                 */
                res.cookie('XSRF-TOKEN', token, {
                    httpOnly: true,
                    secure: true,
                    sameSite: 'strict',
                })
            }
        } catch (error) {
            if (!shouldExcludePathFromCsrf(req.path)) {
                throw new Error('Missing CSRF')
            }
        }

        const parsedUrl = url.parse(req.url, true)
        const { pathname } = parsedUrl

        /**
         * Handle health-check pings
         * Check MVCForum and API services are still running
         */
        if (pathname === '/health-check') {
            const endPoints = [
                {
                    name: 'mvcForum',
                    url: process.env.MVC_FORUM_HEALTH_CHECK_URL,
                },
                {
                    name: 'api',
                    url: process.env.API_HEALTH_CHECK_URL,
                },
            ]

            return Promise.allSettled(
                endPoints.map(({ url }) => {
                    const controller = new AbortController()
                    const timeoutId = setTimeout(() => controller.abort(), 5000)

                    return fetch(url, { signal: controller.signal })
                })
            ).then((responses) => {
                let statusToReturn = 200
                let data = []

                responses?.forEach(({ status, reason, value }, index) => {
                    const metaData = {
                        id: endPoints[index].name,
                        ok: true,
                    }

                    if (status === 'rejected' || (value && !value.ok)) {
                        statusToReturn = 503

                        metaData.ok = false
                        metaData.error = reason || value.statusText
                    }

                    data.push(metaData)
                })

                appInsightsClient?.trackTrace?.(data)
                appInsightsClient?.flush?.()

                return res.status(statusToReturn).json({
                    data: data,
                })
            })
        }

        /**
         * Handle returning the service worker
         */
        if (
            (!isDevelopment && pathname === '/sw.js') ||
            /^\/(workbox|worker|fallback)-\w+\.js$/.test(pathname)
        ) {
            const filePath = join(__dirname, '.next', pathname)
            return res.sendFile(filePath)
        }

        return handle(req, res, parsedUrl)
    })

    /**
     * Handle POST requests
     */
    app.post('*', conditionalCsrf, (req, res) => {
        return handle(req, res)
    })

    /**
     * Start listening for requests
     */
    server = app.listen(process.env.PORT, (error) => {
        if (error) {
            console.log(error)
            throw error
        }

        console.log('Listening on port ' + process.env.PORT)
    })
})
