import Head from 'next/head';
import { GetServerSideProps } from 'next';

import { Layout } from '@components/Layout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { withAuth } from '@hofs/withAuth';
import { selectUser } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

const Index: (props) => JSX.Element = ({
    content,
    user
}) => {

    const { titleText, 
            metaDescriptionText, 
            mainHeadingHtml } = content ?? {};

    return (

        <Layout user={user}>
            <Head>
                <title>{titleText}</title>
                <meta name="description" content={metaDescriptionText} />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn className="u-px-4 u-py-10">
                    <h1>{mainHeadingHtml}</h1>
                </LayoutColumn>
            </LayoutColumnContainer>
        </Layout>

    )

}

export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const user: User = selectUser(context);

    return {
        props: {
            user: user,
            content: {
                titleText: 'Future NHS Home', 
                metaDescriptionText: 'Your Future NHS home page',
                mainHeadingHtml: 'TODO: dashboard'
            }
        }
    }

});

export default Index;