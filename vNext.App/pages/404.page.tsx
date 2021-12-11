import Head from 'next/head';

import { Layout } from '@components/Layout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

const Index: (props) => JSX.Element = ({

}) => {

    return (

        <Layout>
            <Head>
                <title>Page not found</title>
                <meta name="description" content="404: Page not found" />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn className="u-px-4 u-py-10">
                    <h1>404: Page not found</h1>
                </LayoutColumn>
            </LayoutColumnContainer>
        </Layout>

    )

}

export default Index;