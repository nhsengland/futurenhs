import Document, { Html, Head, Main, NextScript } from 'next/document';
import { Response } from 'express';
import { cookiePreferences } from '@constants/cookies';

let cookies: string = undefined;

export default class CustomDocument extends Document {

    static async getInitialProps(ctx) {
            
        const initialProps = await Document.getInitialProps(ctx);

        const { locals } = ctx.res as Response;
        
        const additionalProps = { 
            nonce: locals?.nonce 
        };

        cookies = ctx.req.cookies;
        
        return { 
            ...initialProps,
            ...additionalProps
        }

    }

    render() {

        const { nonce } = (this.props as any);

        const hasAcceptedTrackingCookies: boolean = cookies[cookiePreferences.COOKIE_NAME] === cookiePreferences.ACCEPTED;

        return (

            <Html lang="en">
                <Head nonce={nonce} />
                <body>
                    <Main />
                    <NextScript nonce={nonce} />
                </body>
                {(process.env.NEXT_PUBLIC_GTM_KEY && hasAcceptedTrackingCookies) &&
                    <>
                        <script nonce={nonce} src={`https://www.googletagmanager.com/gtag/js?id=${process.env.NEXT_PUBLIC_GTM_KEY}`} />
                        <script nonce={nonce} dangerouslySetInnerHTML={{
                            __html: `
                                (function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
                                new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
                                j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
                                'https://www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
                                })(window,document,'script','dataLayer', '${process.env.NEXT_PUBLIC_GTM_KEY}');
                            `
                        }} />
                    </>
                }
            </Html>

        )

    }
    
}

