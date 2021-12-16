const isProduction = process.env.NODE_ENV === 'production';
const isDevelopment = process.env.NODE_ENV === 'development';
const isTest = process.env.NODE_ENV === 'test';

/**
 * Import dependencies
 */
const express = require('express');
const next = require('next');
const url = require('url');
const { join } = require('path');
const csrf = require('csurf');
const cookieParser = require('cookie-parser');
const { randomBytes } = require('crypto');
const { AbortController } = require('node-abort-controller');

/**
 * Generate Content Security Policy settings
 */
const generateNonce = () => randomBytes(16).toString('base64');
const generateCSP = (nonce) => {

    const csp = {
        'default-src': isTest ? '*' : `'self' https://assets.nhs.uk https://dc.services.visualstudio.com/v2/track`,
        'img-src': `'self' data:`,
        'style-src': `'self' 'unsafe-inline'`,
        'script-src': `'self' 'nonce-${nonce}'`,
        'script-src-elem': `'self' 'nonce-${nonce}'`,
        'font-src': `'unsafe-inline' https://assets.nhs.uk`
    };
    
    if (isDevelopment) {

        csp['script-src'] = `'self' 'unsafe-eval' 'nonce-${nonce}'`;

    }
  
    return Object.entries(csp).reduce((acc, [key, val]) => `${acc} ${key} ${val};`, '');

};

/**
 * Create an Express app instance
 */
const app = express();

/**
 * Create a CSRF handler
 */
const csrfProtection = csrf({ 
    cookie: {
        secure: true
    }
});

let server = undefined;

/**
 * Bind middleware
 */
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(cookieParser(process.env.COOKIE_PARSER_SECRET));

/**
 * Create a Next.js app instance
 */
const nextApp = next({ 
    dev: isDevelopment 
});
const handle = nextApp.getRequestHandler();

/**
 * Initialise Next
 */
nextApp
    .prepare()
    .then(() => {

        let appInsightsClient;

        /**
         * Start application insights before other dependencies are imported
         */
        if(isProduction && process.env.APPINSIGHTS_INSTRUMENTATIONKEY){

            appInsightsClient = require('applicationinsights');
            appInsightsClient.setup(process.env.APPINSIGHTS_INSTRUMENTATIONKEY).start();

        }

        /**
         * Set response headers
         */
        app.use((req, res, next) => {

            const nonce = generateNonce();
            
            res.set({
                'Cache-Control': 'no-store',                
                'X-DNS-Prefetch-Control': 'on',
                'Strict-Transport-Security': 'max-age=63072000; includeSubDomains; preload',
                'X-XSS-Protection': '1; mode=block',
                'X-Frame-Options': 'SAMEORIGIN',
                'X-Content-Type-Options': 'nosniff',
                'Referrer-Policy': 'origin-when-cross-origin',
                'Content-Security-Policy': generateCSP(nonce)
            });

            res.locals.nonce = nonce;

            next();

        });

        /**
         * Handle GET requests
         */
        app.get('*', csrfProtection, (req, res) => {

            const token = req.csrfToken();
            const parsedUrl = url.parse(req.url, true);
            const { pathname } = parsedUrl;

            /**
             * Set CSRF cookie
             */
            res.cookie('XSRF-TOKEN', token, {
                httpOnly: true,
                secure: true,
                sameSite: 'strict'
            });

            /**
             * Handle health-check pings
             * Check MVCForum and API services are still running
             */
            if(pathname === '/health-check'){

                const endPoints = [
                    {
                        name: 'mvcForum',
                        url: process.env.MVC_FORUM_HEALTH_CHECK_URL
                    }, 
                    {
                        name: 'api',
                        url: process.env.API_HEALTH_CHECK_URL
                    }
                ];
        
                return Promise.allSettled(endPoints.map(({ url }) => {

                    const controller = new AbortController();
                    const timeoutId = setTimeout(() => controller.abort(), 5000);

                    return fetch(url, { signal: controller.signal })

                })).then((responses) => {

                    let statusToReturn = 200;
                    let data = [];

                    responses?.forEach(({ 
                        status, 
                        reason, 
                        value }, index) => {

                            const metaData = {
                                id: endPoints[index].name,
                                ok: true
                            };

                            if(status === 'rejected' || (value && !value.ok)){

                                statusToReturn = 503;

                                metaData.ok = false;
                                metaData.error = reason || value.statusText;

                            }

                            data.push(metaData);

                        });

                    appInsightsClient?.trackTrace?.(data);
                    appInsightsClient?.flush?.();

                    return res.status(statusToReturn).json({
                        data: data
                    });
        
                });

            }

            /**
             * Handle returning the service worker
             */
            if(!isDevelopment && pathname === '/sw.js' || /^\/(workbox|worker|fallback)-\w+\.js$/.test(pathname)) {
                
                const filePath = join(__dirname, '.next', pathname);
                return res.sendFile(filePath);

            }
                
            return handle(req, res, parsedUrl);

        });

        /**
         * Handle POST requests
         */
        app.post('*', csrfProtection, (req, res) => {

            return handle(req, res);

        });

        /**
         * Start listening for requests
         */
        server = app.listen(process.env.PORT, (error) => {

            if (error) {

                console.log(error);
                throw error;
                
            }

            console.log('Listening on port ' + process.env.PORT);

        });    

    });