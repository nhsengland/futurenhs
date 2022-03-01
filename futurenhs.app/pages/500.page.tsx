import Head from 'next/head';

import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

/**
 * Error 500 page
 */
const Index: (props) => JSX.Element = ({

}) => {

    return (

        <>
            <Head>
                <title>Unexpected error</title>
                <meta name="description" content="500: Server error" />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn tablet={8} className="u-py-10">
                    <h1>The FutureNHS website is currently experiencing technical difficulties.</h1>
                    <p className="u-text-lead">We are working to resolve these issues. Please try again later. Thank you for your patience.</p>
                    <p className="u-text-lead">Thank you for your patience.</p>
                </LayoutColumn>
            </LayoutColumnContainer>
        </>

    )

}

export default Index;