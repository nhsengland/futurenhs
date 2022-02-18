import Document, { Html, Head, Main, NextScript } from 'next/document';
import { Response } from 'express';

export default class MyDocument extends Document {

    static async getInitialProps(ctx) {
            
        const initialProps = await Document.getInitialProps(ctx);
        const { locals } = ctx.res as Response;
        const additionalProps = { nonce: locals && locals.nonce };
        
        return { 
            ...initialProps, 
            ...additionalProps
        }

    }

    render() {

        const { nonce } = (this.props as any);

        return (

            <Html lang="en">
                <Head nonce={nonce} />
                <body>
                    <Main />
                    <NextScript nonce={nonce} />
                </body>
            </Html>

        )

    }
    
}
