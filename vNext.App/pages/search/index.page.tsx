import Head from 'next/head';
import Link from 'next/link';
import { GetServerSideProps } from 'next';

import { Layout } from '@components/Layout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { withAuth } from '@hofs/withAuth';

const Index: (props) => JSX.Element = ({
    term,
    content
}) => {

    const { metaDescriptionText, 
            titleText, 
            mainHeadingHtml } = content ?? {};

    return (

        <Layout>
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn desktop={8}>
                    <h1>{`${mainHeadingHtml}: ${term} - 0 Results Found`}</h1>
                    <p>Sorry no results found</p>
                </LayoutColumn>
                <LayoutColumn desktop={4}>
                    <aside>
                        <h2>Groups</h2>
                        <p>
                            <Link href="/groups">View All Groups</Link>
                        </p>
                    </aside>
                </LayoutColumn>
            </LayoutColumnContainer>
        </Layout>

    )

}

export const getServerSideProps: GetServerSideProps = withAuth(async ({ query }) => {

    return {
        props: {
            term: query.term,
            content: {
                metaDescriptionText: 'Search Future NHS',
                titleText: 'Search',
                mainHeadingHtml: 'Searching'
            }
        }
    }

});

export default Index;