const express = require('express');
const next = require('next');
const url = require('url');
const { join } = require('path');
const csrf = require('csurf');
const cookieParser = require('cookie-parser');
const { randomBytes } = require('crypto');

const isDevelopment = process.env.NODE_ENV === 'development';
const isTest = process.env.NODE_ENV === 'test';

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

const app = express();
const csrfProtection = csrf({ 
    cookie: {
        secure: true
    }
});

let server = undefined;

app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(cookieParser(process.env.COOKIE_PARSER_SECRET));

const nextApp = next({ 
    dev: isDevelopment 
});
const handle = nextApp.getRequestHandler();

nextApp
    .prepare()
    .then(() => {

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

        app.get('*', csrfProtection, (req, res) => {

            const token = req.csrfToken();
            const parsedUrl = url.parse(req.url, true);
            const { pathname } = parsedUrl;

            res.cookie('XSRF-TOKEN', token, {
                httpOnly: true,
                secure: true,
                sameSite: 'strict'
            });

            if(pathname === '/health-check'){

                return res.status(200).json({});

            }

            if(!isDevelopment && pathname === '/sw.js' || /^\/(workbox|worker|fallback)-\w+\.js$/.test(pathname)) {
                
                const filePath = join(__dirname, '.next', pathname);
                return res.sendFile(filePath);

            }
                
            return handle(req, res, parsedUrl);

        });

        app.post('*', csrfProtection, (req, res) => {

            return handle(req, res);

        });

        server = app.listen(process.env.PORT, (error) => {

            if (error) {

                console.log(error);
                throw error;
                
            }

            console.log('Listening on port ' + process.env.PORT);

        });
            
    }); 
